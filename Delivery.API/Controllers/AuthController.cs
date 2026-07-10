using System;
using System.Threading.Tasks;
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

        public AuthController(
            ISeguridadService seguridadService,
            IUsuarioService usuarioService,
            IRepartidorService repartidorService,
            IVehiculoService vehiculoService,
            IRolService rolService,
            IRestauranteService restauranteService)
        {
            _seguridadService = seguridadService;
            _usuarioService = usuarioService;
            _repartidorService = repartidorService;
            _vehiculoService = vehiculoService;
            _rolService = rolService;
            _restauranteService = restauranteService;
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var usuario = await _usuarioService.GetByEmailAsync(loginDto.Email);

            if (usuario == null || !usuario.Activo)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });

            var esPasswordValido = _seguridadService.VerificarPassword(loginDto.Password, usuario.PasswordHash);

            if (!esPasswordValido)
                return Unauthorized(new { message = "Credenciales incorrectas." });

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

            // 1. Crear Usuario
            var usuario = new Usuario
            {
                Nombre       = dto.Nombre,
                Apellidos    = dto.Apellidos,
                Email        = dto.Email,
                Telefono     = dto.Telefono,
                Cedula       = dto.Cedula,
                PasswordHash = _seguridadService.HashearPassword(dto.Password),
                RolId        = rolRepartidor.Id,
                TipoUsuario  = TipoUsuarioEnum.Repartidor,
                Activo       = true,
                CreadoEn     = ahora
            };
            var usuarioCreado = await _usuarioService.CreateAsync(usuario);
            if (usuarioCreado == null)
                return StatusCode(500, new { message = "Error al crear el usuario." });

            // 2. Crear Repartidor
            var repartidor = new Repartidor
            {
                UsuarioId        = usuarioCreado.Id,
                LicenciaConducir = dto.LicenciaConducir,
                FotoLicenciaUrl  = !string.IsNullOrEmpty(dto.FotoLicenciaBase64) 
                                   ? $"data:image/jpeg;base64,{dto.FotoLicenciaBase64}" 
                                   : null,
                EstadoAprobacion = EstadoAprobacionEnum.Pendiente,
                Disponible       = false,
                CreadoEn         = ahora
            };
            var repartidorCreado = await _repartidorService.CreateAsync(repartidor);
            if (repartidorCreado == null)
                return StatusCode(500, new { message = "Error al crear el perfil de repartidor." });

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

            // 4. Devolver token para auto-login
            var token = _seguridadService.GenerarTokenJwt(usuarioCreado.Id, usuarioCreado.Email, "Repartidor");
            return Ok(new AuthResponseDto
            {
                Token    = token,
                UsuarioId = usuarioCreado.Id,
                Nombre   = $"{usuarioCreado.Nombre} {usuarioCreado.Apellidos}".Trim(),
                Email    = usuarioCreado.Email,
                Rol      = "Repartidor",
                FotoPerfilUrl = usuarioCreado.FotoPerfilUrl
            });
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

            // 1. Crear Usuario
            var usuario = new Usuario
            {
                Nombre       = dto.NombrePropietario,
                Apellidos    = dto.ApellidosPropietario,
                Email        = dto.Email,
                Telefono     = dto.Telefono,
                Cedula       = dto.Cedula,
                PasswordHash = _seguridadService.HashearPassword(dto.Password),
                RolId        = rolRestaurante.Id,
                TipoUsuario  = TipoUsuarioEnum.Restaurante,
                Activo       = true,
                CreadoEn     = ahora
            };
            var usuarioCreado = await _usuarioService.CreateAsync(usuario);
            if (usuarioCreado == null)
                return StatusCode(500, new { message = "Error al crear el usuario." });

            // 2. Crear Restaurante
            var restaurante = new Restaurante
            {
                Nombre       = dto.NombreRestaurante,
                Descripcion  = dto.Descripcion,
                Categoria    = dto.Categoria,
                Ruc          = dto.RUC,
                Calle        = dto.Calle,
                Ciudad       = dto.Ciudad,
                Telefono     = dto.Telefono,
                Email        = dto.Email,
                HoraApertura = dto.HoraApertura,
                HoraCierre   = dto.HoraCierre,
                Estado       = EstadoRestauranteEnum.Pendiente,
                Abierto      = false,
                CreadoPor    = usuarioCreado.Id,
                CreadoEn     = ahora
            };
            var restauranteCreado = await _restauranteService.CreateAsync(restaurante);
            if (restauranteCreado == null)
                return StatusCode(500, new { message = "Error al crear el perfil de restaurante." });

            // 3. Devolver token para auto-login
            var token = _seguridadService.GenerarTokenJwt(usuarioCreado.Id, usuarioCreado.Email, "Restaurante");
            return Ok(new AuthResponseDto
            {
                Token    = token,
                UsuarioId = usuarioCreado.Id,
                Nombre   = $"{usuarioCreado.Nombre} {usuarioCreado.Apellidos}".Trim(),
                Email    = usuarioCreado.Email,
                Rol      = "Restaurante",
                FotoPerfilUrl = usuarioCreado.FotoPerfilUrl
            });
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

