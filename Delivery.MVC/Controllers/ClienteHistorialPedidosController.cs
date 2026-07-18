using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteHistorialPedidosController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;
        private readonly IResenaConsumer _resenaConsumer;

        public ClienteHistorialPedidosController(IPedidoConsumer pedidoConsumer, IResenaConsumer resenaConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
            _resenaConsumer = resenaConsumer;
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
            var misPedidos = todos.Where(p => p.UsuarioId == userId && p.EstadoPedido != Delivery.Modelos.Enums.EstadoPedidoEnum.Borrador)
                                  .OrderByDescending(p => p.FechaPedido)
                                  .ToList();
            return View(misPedidos);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userId = GetMyUsuarioId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.UsuarioId != userId) return NotFound();
            
            var todas = await _resenaConsumer.GetAllAsync();
            ViewBag.HasResena = todas.Any(r => r.PedidoId == id);
            
            return View(data);
        }

        public async Task<IActionResult> Calificar(long id)
        {
            var userId = GetMyUsuarioId();
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null || pedido.UsuarioId != userId) return NotFound();
            
            if (pedido.EstadoPedido != Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado)
            {
                return RedirectToAction(nameof(Index));
            }

            var todas = await _resenaConsumer.GetAllAsync();
            if (todas.Any(r => r.PedidoId == id))
            {
                TempData["Exito"] = "Ya has calificado este pedido.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var resena = new Resena
            {
                UsuarioId = userId,
                RestauranteId = pedido.RestauranteId,
                RepartidorId = pedido.RepartidorId,
                PedidoId = pedido.Id,
                CalificacionRestaurante = 5, // Default
                CalificacionRepartidor = 5 // Default
            };

            return View(resena);
        }

        [HttpPost]
        public async Task<IActionResult> Calificar(Resena entity)
        {
            var userId = GetMyUsuarioId();
            entity.UsuarioId = userId;
            
            var todas = await _resenaConsumer.GetAllAsync();
            if (todas.Any(r => r.PedidoId == entity.PedidoId))
            {
                return RedirectToAction(nameof(Details), new { id = entity.PedidoId });
            }

            // UML: RateServices()
            var pedido = await _pedidoConsumer.GetByIdAsync(entity.PedidoId);
            if (pedido != null)
            {
                pedido.RateServices((short)(entity.CalificacionRepartidor ?? 5), (short)(entity.CalificacionRestaurante ?? 5));
                
                // UML: opt [HasComment == true]
                if (!string.IsNullOrEmpty(entity.ComentarioRestaurante) || !string.IsNullOrEmpty(entity.ComentarioRepartidor))
                {
                    pedido.SaveReview(entity.ComentarioRestaurante ?? entity.ComentarioRepartidor ?? "");
                }
            }

            await _resenaConsumer.CreateAsync(entity);
            
            // UML: ThankYouMessage()
            TempData["Exito"] = "ThankYouMessage(): ¡Gracias por calificar el servicio!";
            
            return RedirectToAction(nameof(Details), new { id = entity.PedidoId });
        }

        // UML: Track_Order (Activity Diagram)
        public async Task<IActionResult> Seguimiento(long id, [FromServices] IRepartidorConsumer repartidorConsumer)
        {
            var userId = GetMyUsuarioId();
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null || pedido.UsuarioId != userId) return NotFound();

            // UML: Get order status
            ViewBag.OrderStatus = pedido.EstadoPedido.ToString();

            // UML: if In Transit [Si]
            if (pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.EnCamino && pedido.RepartidorId.HasValue)
            {
                // UML: Get driver location -> Request GPS coords
                var driver = await repartidorConsumer.GetByIdAsync(pedido.RepartidorId.Value);
                if (driver != null && driver.UbicacionActual != null)
                {
                    ViewBag.DriverLat = driver.UbicacionActual.Latitud;
                    ViewBag.DriverLng = driver.UbicacionActual.Longitud;
                }
            }
            // else [No] -> Show status and map (handled in view without coords)

            // UML: Show status and map -> View result
            return View(pedido);
        }

        // UML: Cancel Order (Client)
        [HttpPost]
        public async Task<IActionResult> Cancelar(long id)
        {
            var userId = GetMyUsuarioId();
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null || pedido.UsuarioId != userId) return NotFound();

            // UML logic: Can only cancel if it's pending (Pendiente) or similar initial state
            if (pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Pendiente)
            {
                pedido.UpdateStatus("Cancelado");
                await _pedidoConsumer.UpdateAsync(id, pedido);
                TempData["Exito"] = "Pedido cancelado exitosamente.";
            }
            else
            {
                TempData["Error"] = "El pedido ya no puede ser cancelado.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
