using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAsignacionesController : ControllerBase
    {
        private readonly IHistorialAsignacionesRepartidorService _historialService;

        public HistorialAsignacionesController(IHistorialAsignacionesRepartidorService historialService)
        {
            _historialService = historialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAsignacionesRepartidor>>> GetHistorial()
        {
            var historial = await _historialService.GetAllAsync();
            return Ok(historial);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAsignacionesRepartidor>> GetHistorial(long id)
        {
            var historial = await _historialService.GetByIdAsync(id);
            if (historial == null) return NotFound();
            return Ok(historial);
        }

        [HttpPost]
        public async Task<ActionResult<HistorialAsignacionesRepartidor>> PostHistorial(HistorialAsignacionesRepartidor historial)
        {
            var created = await _historialService.CreateAsync(historial);
            return CreatedAtAction(nameof(GetHistorial), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorial(long id, HistorialAsignacionesRepartidor historial)
        {
            if (id != historial.Id) return BadRequest();
            
            var updated = await _historialService.UpdateAsync(historial);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorial(long id)
        {
            var deleted = await _historialService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
