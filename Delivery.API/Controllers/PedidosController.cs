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

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
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
            return Ok(pedido);
        }

        [HttpPut("{id}/estado-repartidor")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Repartidor")]
        public async Task<ActionResult<Pedido>> ActualizarEstadoRepartidor(long id, [FromBody] Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, [FromQuery] long repartidorId)
        {
            var pedido = await _pedidoService.ActualizarEstadoRepartidorAsync(id, nuevoEstado, repartidorId);
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
