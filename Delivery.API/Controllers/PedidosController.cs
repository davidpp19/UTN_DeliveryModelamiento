using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<Delivery.API.Hubs.NotificacionesHub> _hubContext;

        public PedidosController(IPedidoService pedidoService, Microsoft.AspNetCore.SignalR.IHubContext<Delivery.API.Hubs.NotificacionesHub> hubContext)
        {
            _pedidoService = pedidoService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await _pedidoService.GetAllAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(long id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            var created = await _pedidoService.CreateAsync(pedido);
            return CreatedAtAction(nameof(GetPedido), new { id = created.Id }, created);
        }

        /// <summary>
        /// Crea el pedido real desde el carrito de sesión del MVC.
        /// Este es el ÚNICO endpoint que crea registros en la tabla pedidos para el flujo del cliente.
        /// </summary>
        [HttpPost("crear-desde-carrito")]
        public async Task<ActionResult<Pedido>> CrearDesdeCarrito(
            [FromQuery] long usuarioId,
            [FromQuery] long direccionId,
            [FromQuery] Delivery.Modelos.Enums.TipoMetodoPagoEnum metodoPago,
            [FromBody] Delivery.Modelos.DTOs.CarritoSesionDto carritoSesion)
        {
            if (carritoSesion == null || !carritoSesion.Items.Any())
                return BadRequest(new { message = "El carrito está vacío." });

            var pedido = await _pedidoService.CrearDesdeSesionAsync(usuarioId, direccionId, metodoPago, carritoSesion);
            
            // Notificar al restaurante en tiempo real
            await _hubContext.Clients.Group($"Restaurante_{pedido.RestauranteId}").SendAsync("NuevoPedido", pedido.Id);

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(long id, Pedido pedido)
        {
            if (id != pedido.Id) return BadRequest();
            
            var updated = await _pedidoService.UpdateAsync(pedido);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(long id)
        {
            var deleted = await _pedidoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/estado-restaurante")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Restaurante")]
        public async Task<ActionResult<Pedido>> ActualizarEstadoRestaurante(long id, [FromBody] Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, [FromQuery] long restauranteId)
        {
            var pedido = await _pedidoService.ActualizarEstadoRestauranteAsync(id, nuevoEstado, restauranteId);
            
            // Notificar al cliente
            await _hubContext.Clients.Group($"Cliente_{pedido.UsuarioId}").SendAsync("EstadoPedidoCambiado", pedido.Id, nuevoEstado.ToString());
            
            // Si el pedido está listo para recoger, notificar a los repartidores libres
            if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.ListoParaRecoger)
            {
                await _hubContext.Clients.Group("RepartidoresLibres").SendAsync("NuevoPedidoDisponible", pedido.Id);
            }

            return Ok(pedido);
        }

        [HttpPut("{id}/estado-repartidor")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Repartidor")]
        public async Task<ActionResult<Pedido>> ActualizarEstadoRepartidor(long id, [FromBody] Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, [FromQuery] long repartidorId)
        {
            var pedido = await _pedidoService.ActualizarEstadoRepartidorAsync(id, nuevoEstado, repartidorId);
            
            // Notificar al cliente
            await _hubContext.Clients.Group($"Cliente_{pedido.UsuarioId}").SendAsync("EstadoPedidoCambiado", pedido.Id, nuevoEstado.ToString());
            
            return Ok(pedido);
        }

        [HttpPut("{id}/asignar-repartidor")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Repartidor,Admin")]
        public async Task<ActionResult<Pedido>> AsignarRepartidor(long id, [FromQuery] long repartidorId)
        {
            var pedido = await _pedidoService.AsignarPedidoAsync(id, repartidorId);
            
            // Notificar a todos que el pedido ya fue tomado
            await _hubContext.Clients.Group("RepartidoresLibres").SendAsync("PedidoAsignado", pedido.Id);
            
            // Notificar al cliente
            await _hubContext.Clients.Group($"Cliente_{pedido.UsuarioId}").SendAsync("RepartidorAsignado", pedido.Id, repartidorId);
            
            return Ok(pedido);
        }

        [HttpGet("historial/usuario/{usuarioId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetHistorialUsuario(long usuarioId)
        {
            var historial = await _pedidoService.GetHistorialUsuarioAsync(usuarioId);
            return Ok(historial);
        }
    }
}
