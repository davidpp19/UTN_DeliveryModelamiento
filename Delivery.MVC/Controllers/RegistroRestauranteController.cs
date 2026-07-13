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

        public RegistroRestauranteController(IAuthConsumer authConsumer, IStringLocalizer<SharedResource> localizer)
        {
            _authConsumer = authConsumer;
            _localizer = localizer;
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
        public async Task<IActionResult> Index(RegistroRestauranteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            AuthResponseDto response = null;
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

            // Auto-login
            var claims = new System.Collections.Generic.List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, response.Nombre),
                new Claim(ClaimTypes.Email, response.Email),
                new Claim(ClaimTypes.Role, response.Rol),
                new Claim("JwtToken", response.Token)
            };

            if (!string.IsNullOrEmpty(response.FotoPerfilUrl))
            {
                claims.Add(new Claim("FotoPerfilUrl", response.FotoPerfilUrl));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return RedirectToAction("Index", "DashboardRestaurante");
        }
    }
}
