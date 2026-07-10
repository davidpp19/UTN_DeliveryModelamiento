using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAprobacionesController : Controller
    {
        private readonly IAdminAprobacionesConsumer _aprobacionesConsumer;

        public AdminAprobacionesController(IAdminAprobacionesConsumer aprobacionesConsumer)
        {
            _aprobacionesConsumer = aprobacionesConsumer;
        }

        // --- Repartidores ---
        public async Task<IActionResult> Repartidores()
        {
            var pendientes = await _aprobacionesConsumer.GetRepartidoresPendientesAsync();
            return View(pendientes);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRepartidor(long id)
        {
            await _aprobacionesConsumer.AprobarRepartidorAsync(id);
            TempData["Mensaje"] = "Repartidor aprobado.";
            return RedirectToAction(nameof(Repartidores));
        }

        [HttpPost]
        public async Task<IActionResult> RechazarRepartidor(long id, string motivoRechazo)
        {
            await _aprobacionesConsumer.RechazarRepartidorAsync(id, motivoRechazo);
            TempData["Mensaje"] = "Repartidor rechazado.";
            return RedirectToAction(nameof(Repartidores));
        }

        // --- Restaurantes ---
        public async Task<IActionResult> Restaurantes()
        {
            var pendientes = await _aprobacionesConsumer.GetRestaurantesPendientesAsync();
            return View(pendientes);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRestaurante(long id)
        {
            await _aprobacionesConsumer.AprobarRestauranteAsync(id);
            TempData["Mensaje"] = "Restaurante aprobado.";
            return RedirectToAction(nameof(Restaurantes));
        }

        [HttpPost]
        public async Task<IActionResult> RechazarRestaurante(long id, string motivoRechazo)
        {
            await _aprobacionesConsumer.RechazarRestauranteAsync(id, motivoRechazo);
            TempData["Mensaje"] = "Restaurante rechazado.";
            return RedirectToAction(nameof(Restaurantes));
        }
    }
}
