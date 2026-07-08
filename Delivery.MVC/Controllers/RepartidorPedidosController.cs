using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Repartidor,Admin")]
    public class RepartidorPedidosController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;

        public RepartidorPedidosController(IPedidoConsumer pedidoConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
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
            var todos = await _pedidoConsumer.GetAllAsync();
            // Filtrar pedidos donde el RepartidorId coincida con el UsuarioId del repartidor logueado
            var misPedidos = todos.Where(p => p.RepartidorId == userId).OrderByDescending(p => p.FechaPedido);
            return View(misPedidos);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RepartidorId != userId) return NotFound();
            return View(data);
        }

        public async Task<IActionResult> CambiarEstado(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RepartidorId != userId) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(long id, EstadoPedidoEnum nuevoEstado)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RepartidorId != userId) return NotFound();

            var result = await _pedidoConsumer.ActualizarEstadoRepartidorAsync(id, nuevoEstado, userId);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error al actualizar el estado del pedido.");
            return View(data);
        }
    }
}
