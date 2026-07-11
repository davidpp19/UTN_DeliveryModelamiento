using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Cliente,Admin")]
    public class ClienteDireccionesController : Controller
    {
        private readonly IDireccionConsumer _direccionConsumer;

        public ClienteDireccionesController(IDireccionConsumer direccionConsumer)
        {
            _direccionConsumer = direccionConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var todos = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todos.Where(d => d.UsuarioId == userId);
            return View(misDirecciones);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _direccionConsumer.GetByIdAsync(id);
            if (data == null || data.UsuarioId != userId) return NotFound();
            return View(data);
        }

        public IActionResult Create()
        {
            return View(new Direccion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Direccion entity)
        {
            // El UsuarioId siempre viene del usuario autenticado, nunca del formulario
            var userId = GetMyUsuarioId();
            entity.UsuarioId = userId;
            entity.CreadoEn  = DateTime.UtcNow;

            // Eliminar el UsuarioId del ModelState para que no cause error de validación
            ModelState.Remove(nameof(entity.UsuarioId));
            ModelState.Remove(nameof(entity.CreadoEn));

            if (!ModelState.IsValid)
                return View(entity);

            await _direccionConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _direccionConsumer.GetByIdAsync(id);
            if (data == null || data.UsuarioId != userId) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Direccion entity)
        {
            var userId = GetMyUsuarioId();
            if (entity.UsuarioId != userId) return Unauthorized();

            await _direccionConsumer.UpdateAsync(id, entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _direccionConsumer.GetByIdAsync(id);
            if (data == null || data.UsuarioId != userId) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _direccionConsumer.GetByIdAsync(id);
            if (data != null && data.UsuarioId == userId)
            {
                var result = await _direccionConsumer.DeleteAsync(id);
                if (result)
                {
                    TempData["Exito"] = "Dirección eliminada correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se puede eliminar la dirección porque está asociada a uno o más pedidos en el historial.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
