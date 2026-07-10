using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (fechaNacimiento.HasValue) miUsuario.FechaNacimiento = fechaNacimiento;
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
            
            TempData["Exito"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(CompletarPerfil));
        }
    }
}
