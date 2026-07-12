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

        public RegistroRepartidorController(IAuthConsumer authConsumer, IStringLocalizer<SharedResource> localizer)
        {
            _authConsumer = authConsumer;
            _localizer = localizer;
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

            var authResponse = await _authConsumer.RegistroRepartidorAsync(dto);
            if (authResponse != null)
            {
                // Auto-login después del registro
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authResponse.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, authResponse.Nombre),
                    new Claim(ClaimTypes.Email, authResponse.Email),
                    new Claim(ClaimTypes.Role, authResponse.Rol),
                    new Claim("JwtToken", authResponse.Token)
                };

                if (!string.IsNullOrEmpty(authResponse.FotoPerfilUrl))
                {
                    claims.Add(new Claim("FotoPerfilUrl", authResponse.FotoPerfilUrl));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties 
                    { 
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    });

                TempData["Exito"] = _localizer["Registro exitoso. Tu cuenta está pendiente de aprobación por el administrador."];
                return RedirectToAction("Index", "DashboardRepartidor");
            }

            ModelState.AddModelError(string.Empty, _localizer["No se pudo completar el registro. El correo puede ya estar registrado."]);
            return View(dto);
        }
    }
}
