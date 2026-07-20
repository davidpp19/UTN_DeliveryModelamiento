using System.Collections.Generic;
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
    public class RegistroRepartidorController : Controller
    {
        private readonly IAuthConsumer _authConsumer;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly Delivery.MVC.Servicios.IEmailService _emailService;

        public RegistroRepartidorController(IAuthConsumer authConsumer, IStringLocalizer<SharedResource> localizer, Delivery.MVC.Servicios.IEmailService emailService)
        {
            _authConsumer = authConsumer;
            _localizer = localizer;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new RegistroRepartidorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegistroRepartidorDto dto, Microsoft.AspNetCore.Http.IFormFile? fotoLicencia, string? accion = null)
        {
            if (accion == "actualizar")
            {
                ModelState.Clear();
                return View(dto);
            }

            if (!ModelState.IsValid)
                return View(dto);

            // If Bicicleta, we don't need Placa and Licencia, so we can ignore or clear them.
            if (dto.TipoVehiculo == Delivery.Modelos.Enums.TipoVehiculoEnum.Bicicleta)
            {
                dto.LicenciaConducir = "N/A";
                dto.Placa = "N/A";
                dto.FotoLicenciaBase64 = null;
            }
            else
            {
                if (fotoLicencia != null && fotoLicencia.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        await fotoLicencia.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        dto.FotoLicenciaBase64 = System.Convert.ToBase64String(fileBytes);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["La fotografía de la licencia es obligatoria para motos y carros."]);
                    return View(dto);
                }
            }

            AuthResponseDto? authResponse = null;
            try
            {
                authResponse = await _authConsumer.RegistroRepartidorAsync(dto);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, _localizer["Error de la API: " + ex.Message]);
                return View(dto);
            }

            if (authResponse != null)
            {
                if (!string.IsNullOrEmpty(authResponse.CodigoVerificacion))
                {
                    await _emailService.EnviarCorreoConfirmacionAsync(authResponse.Email, authResponse.Nombre, authResponse.CodigoVerificacion);
                    TempData["Mensaje"] = "Revisa tu correo para verificar tu cuenta de repartidor. Una vez verificada, el administrador deberá aprobarla.";
                    return RedirectToAction("VerificarEmail", "Auth", new { email = authResponse.Email });
                }

                TempData["Exito"] = _localizer["Registro exitoso. Tu cuenta está pendiente de aprobación por el administrador."];
                return RedirectToAction("Index", "DashboardRepartidor");
            }

            ModelState.AddModelError(string.Empty, _localizer["No se pudo completar el registro. El correo puede ya estar registrado."]);
            return View(dto);
        }
    }
}
