using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<Pedido>> GetCarrito(long usuarioId)
        {
            var carrito = await _carritoService.ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null) return NotFound(new { message = "No hay un carrito activo para este usuario." });
            
            return Ok(carrito);
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Pedido>> AgregarProducto([FromBody] AgregarAlCarritoDto dto)
        {
            var carrito = await _carritoService.AgregarProductoAsync(dto);
            return Ok(carrito);
        }

        [HttpDelete("{usuarioId}/quitar/{detallePedidoId}")]
        public async Task<IActionResult> QuitarProducto(long usuarioId, long detallePedidoId)
        {
            var resultado = await _carritoService.QuitarProductoAsync(usuarioId, detallePedidoId);
            if (!resultado) return NotFound();

            return NoContent();
        }

        [HttpPost("{usuarioId}/confirmar")]
        public async Task<ActionResult<Pedido>> ConfirmarCarrito(long usuarioId, [FromBody] long direccionId)
        {
            var pedido = await _carritoService.ConfirmarCarritoAsync(usuarioId, direccionId);
            return Ok(pedido);
        }
    }
}
