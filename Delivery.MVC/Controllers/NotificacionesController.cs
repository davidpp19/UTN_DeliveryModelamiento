using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.MVC.Controllers
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        private readonly INotificacionConsumer _notificacionConsumer;

        public NotificacionesController(INotificacionConsumer notificacionConsumer)
        {
            _notificacionConsumer = notificacionConsumer;
        }

        [HttpGet]
        public async Task<IActionResult> GetMisNotificaciones()
        {
            var notificaciones = await _notificacionConsumer.GetMisNotificacionesAsync();
            return Json(notificaciones ?? new System.Collections.Generic.List<Delivery.Modelos.DTOs.NotificacionDto>());
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLeida(long id)
        {
            var result = await _notificacionConsumer.MarcarComoLeidaAsync(id);
            return Json(new { exito = result });
        }
    }
}
