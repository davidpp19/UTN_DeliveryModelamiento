using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;

        public AdminPedidosController(IPedidoConsumer pedidoConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _pedidoConsumer.GetAllAsync();
            return View(data);
        }

        // UML: Cancel Order Admin (Administrator -> Monitor Orders)
        [HttpPost]
        public async Task<IActionResult> Cancelar(long id)
        {
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null) return NotFound();

            pedido.UpdateStatus("Cancelado");
            await _pedidoConsumer.UpdateAsync(id, pedido);
            TempData["Exito"] = "Pedido cancelado por administrador.";

            return RedirectToAction(nameof(Index));
        }

        // UML: Update Order Status (Administrator -> Monitor Orders)
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(long id, string newStatus)
        {
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null) return NotFound();

            if (System.Enum.TryParse<Delivery.Modelos.Enums.EstadoPedidoEnum>(newStatus, true, out var estado))
            {
                pedido.EstadoPedido = estado;
                await _pedidoConsumer.UpdateAsync(id, pedido);
                TempData["Exito"] = "Estado actualizado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
