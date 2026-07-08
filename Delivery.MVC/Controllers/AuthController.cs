using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthConsumer _authConsumer;
        private readonly IUsuarioConsumer _usuarioConsumer;

        public AuthController(IAuthConsumer authConsumer, IUsuarioConsumer usuarioConsumer)
        {
            _authConsumer = authConsumer;
            _usuarioConsumer = usuarioConsumer;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToRoleDashboard();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var authResponse = await _authConsumer.LoginAsync(dto);
            if (authResponse != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authResponse.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, authResponse.Nombre),
                    new Claim(ClaimTypes.Email, authResponse.Email),
                    new Claim(ClaimTypes.Role, authResponse.Rol),
                    new Claim("JwtToken", authResponse.Token) // Guardamos el token por si el frontend lo necesita
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToRoleDashboard(authResponse.Rol);
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToRoleDashboard();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            usuario.TipoUsuario = Delivery.Modelos.Enums.TipoUsuarioEnum.Cliente;
            // Por defecto, asignaremos un rol. En la API debe existir el rol Cliente.
            // Asumiendo que RolId 4 es Cliente o que la API lo maneja.
            usuario.RolId = 4; 
            usuario.Activo = true;

            var created = await _usuarioConsumer.CreateAsync(usuario);
            if (created != null)
            {
                // Auto-login después de registro exitoso
                var loginDto = new LoginDto { Email = usuario.Email, Password = usuario.PasswordHash };
                return await Login(loginDto);
            }

            ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar el usuario.");
            return View(usuario);
        }

        [HttpGet]
        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "El correo es requerido.");
                return View();
            }

            await _authConsumer.RecuperarPasswordAsync(email);
            ViewBag.Message = "Si el correo existe, recibirás instrucciones para recuperar tu contraseña.";
            return View();
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        private IActionResult RedirectToRoleDashboard(string? rol = null)
        {
            rol ??= User.FindFirstValue(ClaimTypes.Role);

            return rol switch
            {
                "Admin" => RedirectToAction("Index", "DashboardAdministrador"),
                "Restaurante" => RedirectToAction("Index", "DashboardRestaurante"),
                "Repartidor" => RedirectToAction("Index", "DashboardRepartidor"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
