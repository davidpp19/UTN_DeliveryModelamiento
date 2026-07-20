using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthConsumer _authConsumer;
        private readonly IUsuarioConsumer _usuarioConsumer;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly Delivery.MVC.Servicios.IEmailService _emailService;

        public AuthController(IAuthConsumer authConsumer, IUsuarioConsumer usuarioConsumer, IStringLocalizer<SharedResource> localizer, Delivery.MVC.Servicios.IEmailService emailService)
        {
            _authConsumer = authConsumer;
            _usuarioConsumer = usuarioConsumer;
            _localizer = localizer;
            _emailService = emailService;
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

            var usuarioDb = await _usuarioConsumer.GetByEmailAsync(dto.Email);
            if (usuarioDb != null && !usuarioDb.EmailConfirmado)
            {
                TempData["Mensaje"] = "Tu cuenta aún no ha sido verificada. Por favor ingresa el código enviado a tu correo.";
                return RedirectToAction("VerificarEmail", new { email = dto.Email });
            }

            try
            {
                var authResponse = await _authConsumer.LoginAsync(dto);
                if (authResponse != null)
                {
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
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToRoleDashboard(authResponse.Rol);
                }

                ModelState.AddModelError(string.Empty, _localizer["Credenciales incorrectas."]);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SeleccionTipoRegistro()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToRoleDashboard();
            return View();
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
        public async Task<IActionResult> Registro(RegistroDto dto)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var codigo = new Random().Next(100000, 999999).ToString();
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellidos = dto.Apellidos,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    Cedula = dto.Cedula,
                    FechaNacimiento = dto.FechaNacimiento,
                    PasswordHash = dto.Password, // Se hashea en la API/Servicio
                    TipoUsuario = Delivery.Modelos.Enums.TipoUsuarioEnum.Cliente,
                    RolId = 4, // Cliente
                    Activo = true,
                    EmailConfirmado = false,
                    CodigoVerificacion = codigo,
                    ExpiracionCodigo = DateTime.UtcNow.AddMinutes(15),
                    CreadoEn = DateTime.UtcNow
                };

                // UML: Validar CalculateAge > 12
                if (usuario.CalculateAge(usuario.DateBirth) <= 12)
                {
                    ModelState.AddModelError(string.Empty, "Debes ser mayor de 12 años para registrarte.");
                    return View(dto);
                }

                // Generar código de verificación de 6 dígitos
                var created = await _usuarioConsumer.CreateAsync(usuario);
                if (created != null)
                {
                    // Enviar correo de verificación por defecto
                    await _emailService.EnviarCorreoConfirmacionAsync(created.Email, created.Nombre, codigo);

                    // Redirigir a verificación
                    TempData["Mensaje"] = "Revisa tu correo para verificar tu cuenta.";
                    return RedirectToAction("VerificarEmail", new { email = created.Email });
                }

                ModelState.AddModelError(string.Empty, _localizer["Ocurrió un error al registrar el usuario. Es posible que el correo o la cédula ya estén registrados."]);
                return View(dto);
            }
            catch (System.Exception ex)
            {
                // Mostramos el error exacto en la vista para depurar en Azure sin la pantalla genérica de error
                ModelState.AddModelError(string.Empty, "Error crítico interno: " + ex.Message + " | Inner: " + ex.InnerException?.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            ViewBag.Email = email;
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario != null)
            {
                ViewBag.Telefono = usuario.Telefono;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarEmail(string email, string codigo)
        {
            ViewBag.Email = email;
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                return View();
            }

            if (usuario.EmailConfirmado)
            {
                TempData["Mensaje"] = "Tu cuenta ya está verificada. Puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            if (usuario.CodigoVerificacion != codigo)
            {
                ModelState.AddModelError(string.Empty, "Código incorrecto.");
                return View();
            }

            if (usuario.ExpiracionCodigo.HasValue && DateTime.UtcNow > usuario.ExpiracionCodigo.Value)
            {
                ModelState.AddModelError(string.Empty, "El código ha expirado. Por favor solicita uno nuevo.");
                return View();
            }

            usuario.EmailConfirmado = true;
            usuario.CodigoVerificacion = null;
            usuario.ExpiracionCodigo = null;

            await _usuarioConsumer.UpdateAsync(usuario.Id, usuario);

            TempData["Mensaje"] = "Cuenta verificada exitosamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> ReenviarCodigo(string email)
        {
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario != null && !usuario.EmailConfirmado)
            {
                var codigo = new Random().Next(100000, 999999).ToString();
                usuario.CodigoVerificacion = codigo;
                usuario.ExpiracionCodigo = DateTime.UtcNow.AddMinutes(15);
                await _usuarioConsumer.UpdateAsync(usuario.Id, usuario);

                await _emailService.EnviarCorreoConfirmacionAsync(usuario.Email, usuario.Nombre, codigo);
            }
            // Retornamos OK incluso si no se encuentra para no filtrar emails existentes
            return Json(new { success = true, mensaje = "Si el correo está registrado, se ha enviado un nuevo código." });
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
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario != null)
            {
                var codigo = new Random().Next(100000, 999999).ToString();
                usuario.CodigoVerificacion = codigo;
                usuario.ExpiracionCodigo = DateTime.UtcNow.AddMinutes(15);
                await _usuarioConsumer.UpdateAsync(usuario.Id, usuario);

                // Enviar correo por defecto
                await _emailService.EnviarCorreoRecuperacionAsync(usuario.Email, usuario.Nombre, codigo);
            }
            
            TempData["Mensaje"] = "Si el correo está registrado, se enviaron instrucciones a tu correo para restablecer tu contraseña.";
            return RedirectToAction("RestablecerPassword", new { email = email });
        }

        // [HttpPost] EnviarSmsRecuperacion removido por Firebase.

        [HttpGet]
        public async Task<IActionResult> RestablecerPassword(string email)
        {
            ViewBag.Email = email;
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario != null)
            {
                ViewBag.Telefono = usuario.Telefono;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerPassword(string email, string codigo, string nuevaContrasena)
        {
            ViewBag.Email = email;
            var usuario = await _usuarioConsumer.GetByEmailAsync(email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Error al verificar el usuario.");
                return View();
            }

            if (usuario.CodigoVerificacion != codigo)
            {
                ModelState.AddModelError(string.Empty, "Código incorrecto.");
                return View();
            }

            if (usuario.ExpiracionCodigo.HasValue && DateTime.UtcNow > usuario.ExpiracionCodigo.Value)
            {
                ModelState.AddModelError(string.Empty, "El código ha expirado. Por favor solicita uno nuevo.");
                return View();
            }

            usuario.PasswordHash = nuevaContrasena;
            usuario.CodigoVerificacion = null;
            usuario.ExpiracionCodigo = null;

            await _usuarioConsumer.UpdateAsync(usuario.Id, usuario);

            TempData["Mensaje"] = "Contraseña restablecida exitosamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerPasswordSms(string telefono, string nuevaContrasena)
        {
            var usuarios = await _usuarioConsumer.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Telefono == telefono);
            
            if (usuario != null)
            {
                usuario.PasswordHash = nuevaContrasena;
                usuario.CodigoVerificacion = null;
                usuario.ExpiracionCodigo = null;

                await _usuarioConsumer.UpdateAsync(usuario.Id, usuario);
                TempData["Mensaje"] = "Contraseña restablecida exitosamente. Ahora puedes iniciar sesión.";
                return Json(new { success = true });
            }
            return Json(new { success = false, mensaje = "Usuario no encontrado." });
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
