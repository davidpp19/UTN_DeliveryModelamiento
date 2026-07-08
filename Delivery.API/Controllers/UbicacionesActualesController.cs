using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UbicacionesActualesController : ControllerBase
    {
        private readonly IUbicacionActualRepartidorService _ubicacionService;

        public UbicacionesActualesController(IUbicacionActualRepartidorService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UbicacionActualRepartidor>>> GetUbicaciones()
        {
            var ubicaciones = await _ubicacionService.GetAllAsync();
            return Ok(ubicaciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UbicacionActualRepartidor>> GetUbicacion(long id)
        {
            var ubicacion = await _ubicacionService.GetByIdAsync(id);
            if (ubicacion == null) return NotFound();
            return Ok(ubicacion);
        }

        [HttpPost]
        public async Task<ActionResult<UbicacionActualRepartidor>> PostUbicacion(UbicacionActualRepartidor ubicacion)
        {
            var created = await _ubicacionService.CreateAsync(ubicacion);
            return CreatedAtAction(nameof(GetUbicacion), new { id = created.RepartidorId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUbicacion(long id, UbicacionActualRepartidor ubicacion)
        {
            if (id != ubicacion.RepartidorId) return BadRequest();
            
            var updated = await _ubicacionService.UpdateAsync(ubicacion);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUbicacion(long id)
        {
            var deleted = await _ubicacionService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
