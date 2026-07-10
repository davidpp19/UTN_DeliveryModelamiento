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

        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var repartidor = await _repartidorConsumer.GetByIdAsync(userId);
            if (repartidor != null && repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Pendiente)
            {
                return RedirectToAction("EnRevision", "Home");
            }
            if (repartidor != null && repartidor.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Rechazado)
            {
                return RedirectToAction("Rechazado", "Home");
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
    }
}
