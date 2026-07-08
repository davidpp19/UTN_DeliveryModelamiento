using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenasController : ControllerBase
    {
        private readonly IResenaService _resenaService;

        public ResenasController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resena>>> GetResenas()
        {
            var resenas = await _resenaService.GetAllAsync();
            return Ok(resenas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resena>> GetResena(long id)
        {
            var resena = await _resenaService.GetByIdAsync(id);
            if (resena == null) return NotFound();
            return Ok(resena);
        }

        [HttpPost]
        public async Task<ActionResult<Resena>> PostResena(Resena resena)
        {
            var created = await _resenaService.CreateAsync(resena);
            return CreatedAtAction(nameof(GetResena), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutResena(long id, Resena resena)
        {
            if (id != resena.Id) return BadRequest();
            
            var updated = await _resenaService.UpdateAsync(resena);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResena(long id)
        {
            var deleted = await _resenaService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
