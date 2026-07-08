using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class RegistroRepartidorController : Controller
    {
        private readonly IAuthConsumer _authConsumer;

        public RegistroRepartidorController(IAuthConsumer authConsumer)
        {
            _authConsumer = authConsumer;
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
        public async Task<IActionResult> Index(RegistroRepartidorDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

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

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true });

                TempData["Exito"] = "Registro exitoso. Tu cuenta está pendiente de aprobación por el administrador.";
                return RedirectToAction("Index", "DashboardRepartidor");
            }

            ModelState.AddModelError(string.Empty, "No se pudo completar el registro. El correo puede ya estar registrado.");
            return View(dto);
        }
    }
}
