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
            return View(data);
        }

        public async Task<IActionResult> Calificar(long id)
        {
            var userId = GetMyUsuarioId();
            var pedido = await _pedidoConsumer.GetByIdAsync(id);
            if (pedido == null || pedido.UsuarioId != userId) return NotFound();
            
            // Solo se puede calificar si está entregado
            if (pedido.EstadoPedido != Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado)
            {
                return RedirectToAction(nameof(Index));
            }

            var resena = new Resena
            {
                UsuarioId = userId,
                RestauranteId = pedido.RestauranteId,
                PedidoId = pedido.Id,
                CalificacionRestaurante = 5 // Default
            };

            return View(resena);
        }

        [HttpPost]
        public async Task<IActionResult> Calificar(Resena entity)
        {
            var userId = GetMyUsuarioId();
            entity.UsuarioId = userId;
            
            await _resenaConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }
    }
}
