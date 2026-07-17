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
    [Authorize(Roles = "Repartidor")]
    public class RepartidorPerfilController : Controller
    {
        private readonly IRepartidorConsumer _repartidorConsumer;
        private readonly IUsuarioConsumer _usuarioConsumer;
        private readonly IArchivoService _archivoService;

        public RepartidorPerfilController(IRepartidorConsumer repartidorConsumer, IUsuarioConsumer usuarioConsumer, IArchivoService archivoService)
        {
            _repartidorConsumer = repartidorConsumer;
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
            var todos = await _repartidorConsumer.GetAllAsync();
            var miRepartidor = todos.FirstOrDefault(r => r.UsuarioId == userId);

            if (miRepartidor == null)
            {
                return RedirectToAction("Index", "DashboardRepartidor");
            }

            return View(miRepartidor);
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPerfil(long id, string nombre, string apellidos, string cedula, string telefono, string datosAdicionales, IFormFile? fotoPerfil)
        {
            var userId = GetMyUsuarioId();
            
            var todos = await _repartidorConsumer.GetAllAsync();
            var miRepartidor = todos.FirstOrDefault(r => r.UsuarioId == id);

            if (miRepartidor == null || miRepartidor.UsuarioId != userId)
            {
                return Unauthorized();
            }

            var miUsuario = await _usuarioConsumer.GetByIdAsync(userId);
            if (miUsuario != null)
            {
                miUsuario.Nombre = nombre;
                miUsuario.Apellidos = apellidos;
                miUsuario.Cedula = cedula;
                miUsuario.Telefono = telefono;
                await _usuarioConsumer.UpdateAsync(userId, miUsuario);
            }

            miRepartidor.DatosAdicionales = datosAdicionales;

            try
            {
                if (fotoPerfil != null)
                {
                    var fotoUrl = await _archivoService.GuardarArchivoAsync(fotoPerfil, "repartidores/perfiles");
                    if (fotoUrl != null)
                    {
                        miUsuario.FotoPerfilUrl = fotoUrl;

                        // Actualizar la cookie de autenticación para que el cambio se refleje inmediatamente
                        if (User.Identity is System.Security.Claims.ClaimsIdentity identity)
                        {
                            var existingClaim = identity.FindFirst("FotoPerfilUrl");
                            if (existingClaim != null) identity.RemoveClaim(existingClaim);
                            identity.AddClaim(new System.Security.Claims.Claim("FotoPerfilUrl", fotoUrl));
                            await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, new System.Security.Claims.ClaimsPrincipal(identity));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(miRepartidor);
            }

            await _repartidorConsumer.UpdateAsync(miRepartidor.UsuarioId, miRepartidor);
            
            TempData["Exito"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(CompletarPerfil));
        }
    }
}
