using System;
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
    [Authorize(Roles = "Cliente")]
    public class ClientePerfilController : Controller
    {
        private readonly IUsuarioConsumer _usuarioConsumer;
        private readonly IArchivoService _archivoService;

        public ClientePerfilController(IUsuarioConsumer usuarioConsumer, IArchivoService archivoService)
        {
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
            var miUsuario = await _usuarioConsumer.GetByIdAsync(userId);

            if (miUsuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(miUsuario);
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPerfil(long id, string nombre, string apellidos, string telefono, DateTime? fechaNacimiento, string informacionAdicional, IFormFile? fotoPerfil)
        {
            var userId = GetMyUsuarioId();
            
            if (id != userId)
            {
                return Unauthorized();
            }

            var miUsuario = await _usuarioConsumer.GetByIdAsync(id);
            if (miUsuario == null)
            {
                return NotFound();
            }

            miUsuario.Nombre = nombre;
            miUsuario.Apellidos = apellidos;
            miUsuario.Telefono = telefono;
            miUsuario.FechaNacimiento = fechaNacimiento;
            miUsuario.InformacionAdicional = informacionAdicional;

            try
            {
                if (fotoPerfil != null)
                {
                    var fotoUrl = await _archivoService.GuardarArchivoAsync(fotoPerfil, "clientes/fotos");
                    if (fotoUrl != null) miUsuario.FotoPerfilUrl = fotoUrl;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(miUsuario);
            }

            await _usuarioConsumer.UpdateAsync(miUsuario.Id, miUsuario);
            
            // Refrescar claim de FotoPerfilUrl
            if (fotoPerfil != null && !string.IsNullOrEmpty(miUsuario.FotoPerfilUrl))
            {
                var authResult = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                if (authResult.Succeeded && authResult.Principal != null)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)authResult.Principal.Identity!;
                    var existingClaim = identity.FindFirst("FotoPerfilUrl");
                    if (existingClaim != null) identity.RemoveClaim(existingClaim);
                    
                    identity.AddClaim(new System.Security.Claims.Claim("FotoPerfilUrl", miUsuario.FotoPerfilUrl));
                    
                    var properties = authResult.Properties ?? new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = true };
                    await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, new System.Security.Claims.ClaimsPrincipal(identity), properties);
                }
            }

            TempData["Exito"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(CompletarPerfil));
        }
    }
}
