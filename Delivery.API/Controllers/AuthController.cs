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

        public AuthController(
            ISeguridadService seguridadService,
            IUsuarioService usuarioService,
            IRepartidorService repartidorService,
            IVehiculoService vehiculoService,
            IRolService rolService)
        {
            _seguridadService = seguridadService;
            _usuarioService = usuarioService;
            _repartidorService = repartidorService;
            _vehiculoService = vehiculoService;
            _rolService = rolService;
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
                Rol = rolNombre
            });
        }

        [HttpPost("registro-repartidor")]
        public async Task<ActionResult<AuthResponseDto>> RegistroRepartidor([FromBody] RegistroRepartidorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar email único
            var existente = await _usuarioService.GetByEmailAsync(dto.Email);
            if (existente != null)
                return BadRequest(new { message = "El correo ya está registrado." });

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
                Rol      = "Repartidor"
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

