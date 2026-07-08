using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepartidoresController : ControllerBase
    {
        private readonly IRepartidorService _repartidorService;

        public RepartidoresController(IRepartidorService repartidorService)
        {
            _repartidorService = repartidorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Repartidor>>> GetRepartidores()
        {
            var repartidores = await _repartidorService.GetAllAsync();
            return Ok(repartidores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Repartidor>> GetRepartidor(long id)
        {
            var repartidor = await _repartidorService.GetByIdAsync(id);
            if (repartidor == null) return NotFound();
            return Ok(repartidor);
        }

        [HttpPost]
        public async Task<ActionResult<Repartidor>> PostRepartidor(Repartidor repartidor)
        {
            var created = await _repartidorService.CreateAsync(repartidor);
            return CreatedAtAction(nameof(GetRepartidor), new { id = created.UsuarioId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepartidor(long id, Repartidor repartidor)
        {
            if (id != repartidor.UsuarioId) return BadRequest();
            
            var updated = await _repartidorService.UpdateAsync(repartidor);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepartidor(long id)
        {
            var deleted = await _repartidorService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
