using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Repartidor")]
    public class DashboardRepartidorController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;
        private readonly IRepartidorConsumer _repartidorConsumer;

        public DashboardRepartidorController(IPedidoConsumer pedidoConsumer, IRepartidorConsumer repartidorConsumer)
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

        public async Task<IActionResult> Index([FromServices] IUsuarioConsumer usuarioConsumer)
        {
            var userId = GetMyUsuarioId();
            
            var notificaciones = await usuarioConsumer.GetNotificacionesAsync(userId);
            ViewBag.Notificaciones = notificaciones.OrderByDescending(n => n.Id).Take(5).ToList();

            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor != null)
            {
                ViewBag.Estado = repartidor.EstadoAprobacion.ToString();
                if (repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Pendiente || 
                    repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Rechazado)
                {
                    return View(); // Return view early without loading operational data
                }
            }

            var todos = await _pedidoConsumer.GetAllAsync();
            var misPedidos = todos.Where(p => p.RepartidorId == userId)
                                  .OrderByDescending(p => p.FechaPedido)
                                  .Take(5)
                                  .ToList();

            ViewBag.Repartidor = repartidor;
            ViewBag.PedidosRecientes = misPedidos;
            ViewBag.TotalEntregados = todos.Count(p => p.RepartidorId == userId && p.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado);
            ViewBag.PedidosActivos = todos.Count(p => p.RepartidorId == userId &&
                p.EstadoPedido != Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado &&
                p.EstadoPedido != Delivery.Modelos.Enums.EstadoPedidoEnum.Cancelado);

            return View();
        }

        // UML: ViewEarnings (Calculate_Commission)
        public async Task<IActionResult> Ganancias()
        {
            var userId = GetMyUsuarioId();
            var todos = await _pedidoConsumer.GetAllAsync();
            var misEntregados = todos.Where(p => p.RepartidorId == userId && p.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado).ToList();

            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor == null) return NotFound();

            double gananciasTotales = 0;
            foreach (var ord in misEntregados)
            {
                // UML: getTotal() on Order
                double return_total = (double)ord.Total;

                // UML: CalculateComission()
                double comisionPorPedido = repartidor.CalculateComission(return_total);
                gananciasTotales += comisionPorPedido;
            }

            // UML: setComission() - The diagram calls it setComission() on itself, 
            // usually meaning it updates its own record of total commissions earned or similar.
            // For now, we calculate it dynamically and pass it to View
            
            ViewBag.GananciasTotales = gananciasTotales;
            ViewBag.TotalPedidos = misEntregados.Count;

            // UML: ShowUpdatedBalance()
            return View(misEntregados);
        }

        // UML: Status Control Form -> Availability Controller
        [HttpPost]
        public async Task<IActionResult> ToggleStatus()
        {
            var userId = GetMyUsuarioId();
            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor == null) return NotFound();

            // UML: Verify Current Status -> Update Delivery Person Table
            if (repartidor.Estado == Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible)
            {
                repartidor.setStatus("Desconectado");
            }
            else if (repartidor.Estado == Delivery.Modelos.Enums.EstadoRepartidorEnum.Desconectado)
            {
                repartidor.setStatus("Disponible");
            }

            await _repartidorConsumer.UpdateAsync(userId, repartidor);

            // UML: Notify Server -> Broadcast Availability Status -> Ok and Return
            TempData["Exito"] = $"Status Control: Tu estado cambió a {repartidor.Estado}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
