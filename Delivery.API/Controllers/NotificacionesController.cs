using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Servicios.Interfaces;
using Delivery.Modelos.DTOs;
using System.Security.Claims;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetMisNotificaciones()
        {
            var userId = GetMyUsuarioId();
            var notificaciones = await _notificacionService.GetNotificacionesByUsuarioAsync(userId);
            return Ok(notificaciones);
        }

        [HttpPost("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(long id)
        {
            var userId = GetMyUsuarioId();
            await _notificacionService.MarcarComoLeidaAsync(id, userId);
            return NoContent();
        }
    }
}
