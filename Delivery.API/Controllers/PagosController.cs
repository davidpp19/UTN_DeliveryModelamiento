using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            var pagos = await _pagoService.GetAllAsync();
            return Ok(pagos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> GetPago(long id)
        {
            var pago = await _pagoService.GetByIdAsync(id);
            if (pago == null) return NotFound();
            return Ok(pago);
        }

        [HttpPost]
        public async Task<ActionResult<Pago>> PostPago(Pago pago)
        {
            var created = await _pagoService.CreateAsync(pago);
            return CreatedAtAction(nameof(GetPago), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPago(long id, Pago pago)
        {
            if (id != pago.Id) return BadRequest();
            
            var updated = await _pagoService.UpdateAsync(pago);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePago(long id)
        {
            var deleted = await _pagoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
