using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISeguridadService _seguridadService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRepartidorService _repartidorService;
        private readonly IVehiculoService _vehiculoService;
        private readonly IRolService _rolService;
        private readonly IRestauranteService _restauranteService;
        private readonly Delivery.Modelos.DeliveryDbContext _context;

        public AuthController(
            ISeguridadService seguridadService,
            IUsuarioService usuarioService,
            IRepartidorService repartidorService,
            IVehiculoService vehiculoService,
            IRolService rolService,
            IRestauranteService restauranteService,
            Delivery.Modelos.DeliveryDbContext context)
        {
            _seguridadService = seguridadService;
            _usuarioService = usuarioService;
            _repartidorService = repartidorService;
            _vehiculoService = vehiculoService;
            _rolService = rolService;
            _restauranteService = restauranteService;
            _context = context;
        }

        [HttpGet("forzar-verificacion")]
        public async Task<IActionResult> ForzarVerificacion()
        {
            try
            {
                // Fix 1: Always verify admin/test emails
                var emailsAConfirmar = new[] { "admin@rayoexpres.com", "davidtomas@gmail.com", "admin@admin.com" };
                var usuariosAdmin = await _context.Usuarios
                    .Where(u => emailsAConfirmar.Contains(u.Email))
                    .ToListAsync();

                foreach (var user in usuariosAdmin)
                {
                    user.EmailConfirmado = true;
                    user.IntentosFallidos = 0;
                    user.BloqueadoHasta = null;
                }

                // Fix 2: Also confirm ALL repartidores/restaurantes that are already approved
                // These were blocked from login because approval didn't mark EmailConfirmado=true
                var repartidoresAprobados = await _context.Repartidores
                    .Where(r => r.EstadoAprobacion == EstadoAprobacionEnum.Aprobado)
                    .Include(r => r.Usuario)
                    .ToListAsync();

                foreach (var rep in repartidoresAprobados)
                {
                    if (rep.Usuario != null && !rep.Usuario.EmailConfirmado)
                    {
                        rep.Usuario.EmailConfirmado = true;
                        rep.Usuario.IntentosFallidos = 0;
                        rep.Usuario.BloqueadoHasta = null;
                        rep.Usuario.CodigoVerificacion = null;
                        rep.Usuario.ExpiracionCodigo = null;
                    }
                }

                var restaurantesAprobados = await _context.Restaurantes
                    .Where(r => r.Estado == EstadoRestauranteEnum.Aprobado && r.CreadoPor != null)
                    .ToListAsync();

                foreach (var rest in restaurantesAprobados)
                {
                    if (rest.CreadoPor.HasValue)
                    {
                        var usuarioRest = await _context.Usuarios.FindAsync(rest.CreadoPor.Value);
                        if (usuarioRest != null && !usuarioRest.EmailConfirmado)
                        {
                            usuarioRest.EmailConfirmado = true;
                            usuarioRest.IntentosFallidos = 0;
                            usuarioRest.BloqueadoHasta = null;
                            usuarioRest.CodigoVerificacion = null;
                            usuarioRest.ExpiracionCodigo = null;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Verificacion masiva completada.",
                    adminsActualizados = usuariosAdmin.Count,
                    repartidoresActualizados = repartidoresAprobados.Count,
                    restaurantesActualizados = restaurantesAprobados.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al verificar cuentas.", error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var usuario = await _usuarioService.GetByEmailAsync(loginDto.Email);

            if (usuario == null || !usuario.Activo)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > DateTime.UtcNow)
            {
                var minutosRestantes = (int)(usuario.BloqueadoHasta.Value - DateTime.UtcNow).TotalMinutes;
                return StatusCode(403, new { message = $"Su cuenta está bloqueada por demasiados intentos fallidos. Intente de nuevo en {minutosRestantes} minutos o póngase en contacto con soporte." });
            }

            var esPasswordValido = _seguridadService.VerificarPassword(loginDto.Password, usuario.PasswordHash);

            if (!esPasswordValido)
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= 5)
                {
                    usuario.BloqueadoHasta = DateTime.UtcNow.AddMinutes(15);
                    await _usuarioService.UpdateAsync(usuario);
                    return StatusCode(403, new { message = "Ha superado el número máximo de intentos. Su cuenta ha sido bloqueada por 15 minutos." });
                }
                
                await _usuarioService.UpdateAsync(usuario);
                return Unauthorized(new { message = $"Credenciales incorrectas. Le quedan {5 - usuario.IntentosFallidos} intentos." });
            }

            // Reseteo de intentos
            if (usuario.IntentosFallidos > 0 || usuario.BloqueadoHasta.HasValue)
            {
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta = null;
                await _usuarioService.UpdateAsync(usuario);
            }

            var rolNombre = usuario.Rol?.Nombre ?? "Cliente";
            var token = _seguridadService.GenerarTokenJwt(usuario.Id, usuario.Email, rolNombre);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UsuarioId = usuario.Id,
                Nombre = $"{usuario.Nombre} {usuario.Apellidos}".Trim(),
                Email = usuario.Email,
                Rol = rolNombre,
                FotoPerfilUrl = usuario.FotoPerfilUrl
            });
        }

        [HttpPost("registro-repartidor")]
        public async Task<ActionResult<AuthResponseDto>> RegistroRepartidor([FromBody] RegistroRepartidorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _usuarioService.GetByEmailAsync(dto.Email);
            if (existente != null)
                return BadRequest(new { message = "El correo ya está registrado." });

            var todos = await _usuarioService.GetAllAsync();
            if (System.Linq.Enumerable.Any(todos, u => u.Cedula == dto.Cedula))
                return BadRequest(new { message = "Ya existe un usuario registrado con esa cédula." });

            // Validar Placa y Licencia únicas si no es bicicleta
            if (dto.TipoVehiculo != TipoVehiculoEnum.Bicicleta)
            {
                var vehiculos = await _vehiculoService.GetAllAsync();
                if (vehiculos.Any(v => v.Placa == dto.Placa))
                    return BadRequest(new { message = "La placa ya está registrada en el sistema." });

                var repartidores = await _repartidorService.GetAllAsync();
                if (repartidores.Any(r => r.LicenciaConducir == dto.LicenciaConducir))
                    return BadRequest(new { message = "La licencia ya está registrada en el sistema." });
            }

            // Obtener rol Repartidor
            var roles = await _rolService.GetAllAsync();
            var rolRepartidor = System.Linq.Enumerable.FirstOrDefault(roles, r => r.Nombre == "Repartidor");
            if (rolRepartidor == null)
                return StatusCode(500, new { message = "El rol Repartidor no está configurado en el sistema." });

            var ahora = DateTime.UtcNow;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var codigo = new Random().Next(100000, 999999).ToString();
                // 1. Crear Usuario
                var usuario = new Usuario
                {
                    Nombre       = dto.Nombre,
                    Apellidos    = dto.Apellidos,
                    Email        = dto.Email,
                    Telefono     = dto.Telefono,
                    Cedula       = dto.Cedula,
                    PasswordHash = dto.Password,
                    RolId        = rolRepartidor.Id,
                    TipoUsuario  = TipoUsuarioEnum.Repartidor,
                    Activo       = true,
                    EmailConfirmado = false,
                    CodigoVerificacion = codigo,
                    ExpiracionCodigo = DateTime.UtcNow.AddMinutes(15),
                    CreadoEn     = ahora
                };
                var usuarioCreado = await _usuarioService.CreateAsync(usuario);
                if (usuarioCreado == null)
                    throw new Exception("Error al crear el usuario.");

                // 2. Crear Repartidor
                var repartidor = new Repartidor
                {
                    UsuarioId        = usuarioCreado.Id,
                    LicenciaConducir = dto.LicenciaConducir,
                    FotoLicenciaUrl  = !string.IsNullOrEmpty(dto.FotoLicenciaBase64) 
                                       ? $"data:image/jpeg;base64,{dto.FotoLicenciaBase64}" 
                                       : null,
                    EstadoAprobacion = EstadoAprobacionEnum.Pendiente,
                    Estado           = EstadoRepartidorEnum.Desconectado,
                    CreadoEn         = ahora
                };
                var repartidorCreado = await _repartidorService.CreateAsync(repartidor);
                if (repartidorCreado == null)
                    throw new Exception("Error al crear el perfil de repartidor.");

                // 3. Crear Vehículo
                var vehiculo = new Vehiculo
                {
                    RepartidorId = usuarioCreado.Id,
                    TipoVehiculo = dto.TipoVehiculo,
                    Placa        = dto.Placa,
                    Marca        = dto.Marca,
                    Modelo       = dto.Modelo,
                    Color        = dto.Color,
                    Anio         = dto.Anio,
                    CreadoEn     = ahora
                };
                await _vehiculoService.CreateAsync(vehiculo);

                await transaction.CommitAsync();

                // 4. Devolver token para auto-login
                var token = _seguridadService.GenerarTokenJwt(usuarioCreado.Id, usuarioCreado.Email, "Repartidor");
                return Ok(new AuthResponseDto
                {
                    Token    = token,
                    UsuarioId = usuarioCreado.Id,
                    Nombre   = $"{usuarioCreado.Nombre} {usuarioCreado.Apellidos}".Trim(),
                    Email    = usuarioCreado.Email,
                    Rol      = "Repartidor",
                    FotoPerfilUrl = usuarioCreado.FotoPerfilUrl,
                    CodigoVerificacion = codigo
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error interno durante el registro: " + ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("registro-restaurante")]
        public async Task<ActionResult<AuthResponseDto>> RegistroRestaurante([FromBody] RegistroRestauranteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _usuarioService.GetByEmailAsync(dto.Email);
            if (existente != null)
                return BadRequest(new { message = "El correo ya está registrado." });

            var todos = await _usuarioService.GetAllAsync();
            if (System.Linq.Enumerable.Any(todos, u => u.Cedula == dto.Cedula))
                return BadRequest(new { message = "Ya existe un usuario registrado con esa cédula." });

            // Verificar RUC único
            var restaurantes = await _restauranteService.GetAllAsync();
            if (restaurantes.Any(r => r.Ruc == dto.RUC))
                return BadRequest(new { message = "El RUC ya está registrado." });

            // Obtener rol Restaurante
            var roles = await _rolService.GetAllAsync();
            var rolRestaurante = System.Linq.Enumerable.FirstOrDefault(roles, r => r.Nombre == "Restaurante");
            if (rolRestaurante == null)
                return StatusCode(500, new { message = "El rol Restaurante no está configurado en el sistema." });

            var ahora = DateTime.UtcNow;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var codigo = new Random().Next(100000, 999999).ToString();
                // 1. Crear Usuario
                var usuario = new Usuario
                {
                    Nombre       = dto.NombrePropietario,
                    Apellidos    = dto.ApellidosPropietario,
                    Email        = dto.Email,
                    Telefono     = dto.Telefono,
                    Cedula       = dto.Cedula,
                    PasswordHash = dto.Password,
                    RolId        = rolRestaurante.Id,
                    TipoUsuario  = TipoUsuarioEnum.Restaurante,
                    Activo       = true,
                    EmailConfirmado = false,
                    CodigoVerificacion = codigo,
                    ExpiracionCodigo = DateTime.UtcNow.AddMinutes(15),
                    CreadoEn     = ahora
                };
                var usuarioCreado = await _usuarioService.CreateAsync(usuario);
                if (usuarioCreado == null)
                    throw new Exception("Error al crear el usuario.");

                // 2. Crear Restaurante
                var restaurante = new Restaurante
                {
                    Nombre       = dto.NombreRestaurante,
                    Descripcion  = dto.Descripcion,
                    Categoria    = dto.Categoria,
                    Ruc          = dto.RUC,
                    Calle        = dto.Calle,
                    Ciudad       = dto.Ciudad,
                    Latitud      = dto.Latitud,
                    Longitud     = dto.Longitud,
                    Telefono     = dto.Telefono,
                    Email        = dto.Email,
                    HoraApertura = dto.HoraApertura,
                    HoraCierre   = dto.HoraCierre,
                    Estado       = EstadoRestauranteEnum.Pendiente,
                    Abierto      = true,
                    CreadoPor    = usuarioCreado.Id,
                    CreadoEn     = ahora
                };
                var restauranteCreado = await _restauranteService.CreateAsync(restaurante);
                if (restauranteCreado == null)
                    throw new Exception("Error al crear el perfil de restaurante.");

                await transaction.CommitAsync();

                // 3. Devolver token para auto-login
                var token = _seguridadService.GenerarTokenJwt(usuarioCreado.Id, usuarioCreado.Email, "Restaurante");
                return Ok(new AuthResponseDto
                {
                    Token    = token,
                    UsuarioId = usuarioCreado.Id,
                    Nombre   = $"{usuarioCreado.Nombre} {usuarioCreado.Apellidos}".Trim(),
                    Email    = usuarioCreado.Email,
                    Rol      = "Restaurante",
                    FotoPerfilUrl = usuarioCreado.FotoPerfilUrl,
                    CodigoVerificacion = codigo
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error interno durante el registro: " + ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] string email)
        {
            var usuario = await _usuarioService.GetByEmailAsync(email);
            // Para evitar User Enumeration, siempre OK
            return Ok(new { message = "Si el correo existe, recibirá instrucciones para recuperar su contraseña." });
        }
    }
}

