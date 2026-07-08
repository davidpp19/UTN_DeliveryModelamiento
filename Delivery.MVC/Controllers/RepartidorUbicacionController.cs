using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;
using System;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Repartidor")]
    public class RepartidorUbicacionController : Controller
    {
        private readonly IUbicacionActualRepartidorConsumer _ubicacionConsumer;

        public RepartidorUbicacionController(IUbicacionActualRepartidorConsumer ubicacionConsumer)
        {
            _ubicacionConsumer = ubicacionConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var todos = await _ubicacionConsumer.GetAllAsync();
            var miUbicacion = todos.FirstOrDefault(u => u.RepartidorId == userId);
            
            if (miUbicacion == null)
            {
                miUbicacion = new UbicacionActualRepartidor { RepartidorId = userId };
            }
            return View(miUbicacion);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation(UbicacionActualRepartidor entity)
        {
            var userId = GetMyUsuarioId();
            entity.RepartidorId = userId;
            entity.ActualizadoEn = DateTime.UtcNow;

            var todos = await _ubicacionConsumer.GetAllAsync();
            var miUbicacion = todos.FirstOrDefault(u => u.RepartidorId == userId);

            if (miUbicacion == null)
            {
                await _ubicacionConsumer.CreateAsync(entity);
            }
            else
            {
                await _ubicacionConsumer.UpdateAsync(miUbicacion.RepartidorId, entity);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
