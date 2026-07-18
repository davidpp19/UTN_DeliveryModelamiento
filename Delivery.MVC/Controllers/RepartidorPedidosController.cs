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
        private readonly IRepartidorConsumer _repartidorConsumer;

        public RepartidorPedidosController(IPedidoConsumer pedidoConsumer, IRepartidorConsumer repartidorConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
            _repartidorConsumer = repartidorConsumer;
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
            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor != null && (repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Pendiente || 
                                       repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Rechazado))
            {
                return RedirectToAction("Index", "DashboardRepartidor");
            }

            var todos = await _pedidoConsumer.GetAllAsync();
            var misPedidos = todos.Where(p => p.RepartidorId == userId && 
                p.EstadoPedido != EstadoPedidoEnum.Entregado && 
                p.EstadoPedido != EstadoPedidoEnum.Cancelado);
            return View(misPedidos);
        }

        public async Task<IActionResult> Disponibles()
        {
            var userId = GetMyUsuarioId();
            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor != null && (repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Pendiente || 
                                       repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Rechazado))
            {
                return RedirectToAction("Index", "DashboardRepartidor");
            }
            
            var todos = await _pedidoConsumer.GetAllAsync();
            // Mostrar pedidos que no tengan repartidor y que no estén cancelados ni entregados
            var pedidosListos = todos.Where(p => p.RepartidorId == null && 
                                                 p.EstadoPedido != EstadoPedidoEnum.Cancelado && 
                                                 p.EstadoPedido != EstadoPedidoEnum.Entregado)
                                     .OrderByDescending(p => p.FechaPedido);
            return View(pedidosListos);
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidoInfo(long id)
        {
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Json(new {
                id = data.Id,
                restaurante = data.Restaurante?.Nombre ?? "Desconocido",
                costoEnvio = data.CostoEnvio.ToString("0.00"),
                total = data.Total.ToString("0.00")
            });
        }

        [HttpPost]
        public async Task<IActionResult> AceptarPedido(long id)
        {
            var userId = GetMyUsuarioId();
            try
            {
                // UML: getStatus() and verify status == "Pending"
                var pedido = await _pedidoConsumer.GetByIdAsync(id);
                if (pedido != null)
                {
                    if (pedido.EstadoPedido.ToString() == "Pendiente" || pedido.EstadoPedido == EstadoPedidoEnum.Pendiente)
                    {
                        var result = await _pedidoConsumer.AsignarPedidoAsync(id, userId);
                        if (result != null)
                        {
                            TempData["Exito"] = "Pedido asignado exitosamente.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        // UML: OrderAlreadyTaken()
                        TempData["Error"] = "OrderAlreadyTaken: El pedido ya fue asignado a otro repartidor.";
                        return RedirectToAction(nameof(Disponibles));
                    }
                }
            }
            catch (System.Exception)
            {
                TempData["Error"] = "OrderAlreadyTaken: El pedido ya fue asignado o hubo un error.";
            }

            return RedirectToAction(nameof(Disponibles));
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

            var estadosPermitidos = new System.Collections.Generic.List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            switch (data.EstadoPedido)
            {
                case EstadoPedidoEnum.ListoParaRecoger:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Recogido", Value = "Recogido" });
                    break;
                case EstadoPedidoEnum.Recogido:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "En Camino", Value = "EnCamino" });
                    break;
                case EstadoPedidoEnum.EnCamino:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Entregado", Value = "Entregado" });
                    break;
            }
            ViewBag.EstadosPermitidos = estadosPermitidos;

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

        public async Task<IActionResult> Cancelar(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RepartidorId != userId) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CancelarConfirmado(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RepartidorId != userId) return NotFound();

            // Lógica de cancelación: se libera el pedido
            data.RepartidorId = null;
            data.EstadoPedido = EstadoPedidoEnum.Pendiente;

            var result = await _pedidoConsumer.UpdateAsync(id, data);
            if (result)
            {
                // TODO: Notificar vía SignalR que hay un nuevo pedido disponible
                TempData["Exito"] = "Has cancelado la entrega. El pedido volverá a estar disponible para otros repartidores.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al cancelar la entrega.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
