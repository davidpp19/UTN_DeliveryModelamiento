using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class RegistroRestauranteController : Controller
    {
        private readonly IAuthConsumer _authConsumer;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly Delivery.MVC.Servicios.IEmailService _emailService;

        public RegistroRestauranteController(IAuthConsumer authConsumer, IStringLocalizer<SharedResource> localizer, Delivery.MVC.Servicios.IEmailService emailService)
        {
            _authConsumer = authConsumer;
            _localizer = localizer;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegistroRestauranteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegistroRestauranteDto dto, string? latStr, string? lngStr)
        {
            if (!string.IsNullOrEmpty(latStr) && decimal.TryParse(latStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal lat))
            {
                dto.Latitud = lat;
                ModelState.Remove("Latitud");
            }
            if (!string.IsNullOrEmpty(lngStr) && decimal.TryParse(lngStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal lng))
            {
                dto.Longitud = lng;
                ModelState.Remove("Longitud");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            AuthResponseDto? response = null;
            try
            {
                response = await _authConsumer.RegistroRestauranteAsync(dto);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, _localizer["Error de la API: " + ex.Message]);
                return View(dto);
            }

            if (response == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["Hubo un error en el registro. No se recibió respuesta."]);
                return View(dto);
            }

            if (!string.IsNullOrEmpty(response.CodigoVerificacion))
            {
                await _emailService.EnviarCorreoConfirmacionAsync(response.Email, response.Nombre, response.CodigoVerificacion);
                TempData["Mensaje"] = "Revisa tu correo para verificar tu cuenta de restaurante. Una vez verificada, el administrador deberá aprobarla.";
                return RedirectToAction("VerificarEmail", "Auth", new { email = response.Email });
            }

            return RedirectToAction("Index", "DashboardRestaurante");
        }
    }
}
