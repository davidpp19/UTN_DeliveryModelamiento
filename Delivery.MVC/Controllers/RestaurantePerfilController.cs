using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Delivery.Consumer.Interfaces;
using Delivery.MVC.Servicios;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Restaurante")]
    public class RestaurantePerfilController : Controller
    {
        private readonly IRestauranteConsumer _restauranteConsumer;
        private readonly IUsuarioConsumer _usuarioConsumer;
        private readonly IArchivoService _archivoService;

        public RestaurantePerfilController(IRestauranteConsumer restauranteConsumer, IUsuarioConsumer usuarioConsumer, IArchivoService archivoService)
        {
            _restauranteConsumer = restauranteConsumer;
            _usuarioConsumer = usuarioConsumer;
            _archivoService = archivoService;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> CompletarPerfil()
        {
            var userId = GetMyUsuarioId();
            var todos = await _restauranteConsumer.GetAllAsync();
            var miRestaurante = todos.FirstOrDefault(r => r.CreadoPor == userId);

            if (miRestaurante == null)
            {
                return RedirectToAction("Index", "DashboardRestaurante");
            }

            return View(miRestaurante);
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPerfil(long id, string descripcion, string telefono, string email, TimeSpan? horaApertura, TimeSpan? horaCierre, IFormFile? logo, IFormFile? portada, string redesSociales)
        {
            var userId = GetMyUsuarioId();
            var miRestaurante = await _restauranteConsumer.GetByIdAsync(id);

            if (miRestaurante == null || miRestaurante.CreadoPor != userId)
            {
                return Unauthorized();
            }

            miRestaurante.Descripcion = descripcion;
            miRestaurante.Telefono = telefono;
            miRestaurante.Email = email;
            if (horaApertura.HasValue) miRestaurante.HoraApertura = horaApertura;
            if (horaCierre.HasValue) miRestaurante.HoraCierre = horaCierre;
            miRestaurante.RedesSociales = redesSociales;

            try
            {
                if (logo != null)
                {
                    var logoUrl = await _archivoService.GuardarArchivoAsync(logo, "restaurantes/logos");
                    if (logoUrl != null)
                    {
                        miRestaurante.LogoUrl = logoUrl;
                        // También actualizar la foto de perfil del usuario para el menú superior
                        var miUsuario = await _usuarioConsumer.GetByIdAsync(userId);
                        if (miUsuario != null)
                        {
                            miUsuario.FotoPerfilUrl = logoUrl;
                            await _usuarioConsumer.UpdateAsync(userId, miUsuario);

                            // Actualizar la cookie de autenticación para que el cambio se refleje inmediatamente
                            var authResult = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                            if (authResult.Succeeded && authResult.Principal != null)
                            {
                                var identity = (System.Security.Claims.ClaimsIdentity)authResult.Principal.Identity!;
                                var existingClaim = identity.FindFirst("FotoPerfilUrl");
                                if (existingClaim != null) identity.RemoveClaim(existingClaim);
                                identity.AddClaim(new System.Security.Claims.Claim("FotoPerfilUrl", logoUrl));
                                
                                var properties = authResult.Properties ?? new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = true };
                                await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, new System.Security.Claims.ClaimsPrincipal(identity), properties);
                            }
                        }
                    }
                }

                if (portada != null)
                {
                    var portadaUrl = await _archivoService.GuardarArchivoAsync(portada, "restaurantes/portadas");
                    if (portadaUrl != null) miRestaurante.PortadaUrl = portadaUrl;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(miRestaurante);
            }

            await _restauranteConsumer.UpdateAsync(miRestaurante.Id, miRestaurante);
            
            TempData["Exito"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(CompletarPerfil));
        }
    }
}
