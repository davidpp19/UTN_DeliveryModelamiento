using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesPedidoController : ControllerBase
    {
        private readonly IDetallePedidoService _detalleService;

        public DetallesPedidoController(IDetallePedidoService detalleService)
        {
            _detalleService = detalleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
        {
            var detalles = await _detalleService.GetAllAsync();
            return Ok(detalles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetallePedido>> GetDetalle(long id)
        {
            var detalle = await _detalleService.GetByIdAsync(id);
            if (detalle == null) return NotFound();
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<ActionResult<DetallePedido>> PostDetalle(DetallePedido detalle)
        {
            var created = await _detalleService.CreateAsync(detalle);
            return CreatedAtAction(nameof(GetDetalle), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalle(long id, DetallePedido detalle)
        {
            if (id != detalle.Id) return BadRequest();
            
            var updated = await _detalleService.UpdateAsync(detalle);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalle(long id)
        {
            var deleted = await _detalleService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
