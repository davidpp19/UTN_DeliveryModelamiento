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
    }
}
