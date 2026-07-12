using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAprobacionesController : Controller
    {
        private readonly IAdminAprobacionesConsumer _aprobacionesConsumer;
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

        public AdminAprobacionesController(IAdminAprobacionesConsumer aprobacionesConsumer, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
        {
            _aprobacionesConsumer = aprobacionesConsumer;
            _localizer = localizer;
        }

        // --- Repartidores ---
        public async Task<IActionResult> Repartidores()
        {
            try 
            {
                var pendientes = await _aprobacionesConsumer.GetRepartidoresPendientesAsync();
                return View(pendientes);
            }
            catch (System.Net.Http.HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Si la API devuelve 401, el token JWT expiró o no existe. Forzar re-login.
                await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRepartidor(long id)
        {
            await _aprobacionesConsumer.AprobarRepartidorAsync(id);
            TempData["Mensaje"] = _localizer["RepartidorAprobadoMsj"].Value;
            return RedirectToAction(nameof(Repartidores));
        }

        [HttpPost]
        public async Task<IActionResult> RechazarRepartidor(long id, string motivoRechazo)
        {
            await _aprobacionesConsumer.RechazarRepartidorAsync(id, motivoRechazo);
            TempData["Mensaje"] = _localizer["RepartidorRechazadoMsj"].Value;
            return RedirectToAction(nameof(Repartidores));
        }

        // --- Restaurantes ---
        public async Task<IActionResult> Restaurantes()
        {
            try 
            {
                var pendientes = await _aprobacionesConsumer.GetRestaurantesPendientesAsync();
                return View(pendientes);
            }
            catch (System.Net.Http.HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRestaurante(long id)
        {
            await _aprobacionesConsumer.AprobarRestauranteAsync(id);
            TempData["Mensaje"] = _localizer["RestauranteAprobadoMsj"].Value;
            return RedirectToAction(nameof(Restaurantes));
        }

        [HttpPost]
        public async Task<IActionResult> RechazarRestaurante(long id, string motivoRechazo)
        {
            await _aprobacionesConsumer.RechazarRestauranteAsync(id, motivoRechazo);
            TempData["Mensaje"] = _localizer["RestauranteRechazadoMsj"].Value;
            return RedirectToAction(nameof(Restaurantes));
        }
    }
}
