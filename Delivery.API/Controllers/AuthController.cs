using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISeguridadService _seguridadService;
        private readonly IUsuarioService _usuarioService;

        public AuthController(ISeguridadService seguridadService, IUsuarioService usuarioService)
        {
            _seguridadService = seguridadService;
            _usuarioService = usuarioService;
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // El logout con JWT se maneja del lado del cliente borrando el token.
            // Aquí solo retornamos OK.
            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] string email)
        {
            var usuario = await _usuarioService.GetByEmailAsync(email);
            if (usuario == null)
            {
                // Para evitar User Enumeration, retornamos OK de todas formas.
                return Ok(new { message = "Si el correo existe, recibirá instrucciones para recuperar su contraseña." });
            }

            // Aquí se integraría INotificacionService para enviar el correo con un link temporal
            // await _notificacionService.EnviarNotificacionAsync(new MensajeNotificacionDto { ... });

            return Ok(new { message = "Si el correo existe, recibirá instrucciones para recuperar su contraseña." });
        }
    }
}
