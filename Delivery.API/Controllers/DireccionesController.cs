using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionesController : ControllerBase
    {
        private readonly IDireccionService _direccionService;

        public DireccionesController(IDireccionService direccionService)
        {
            _direccionService = direccionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Direccion>>> GetDirecciones()
        {
            var direcciones = await _direccionService.GetAllAsync();
            return Ok(direcciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Direccion>> GetDireccion(long id)
        {
            var direccion = await _direccionService.GetByIdAsync(id);
            if (direccion == null) return NotFound();
            return Ok(direccion);
        }

        [HttpPost]
        public async Task<ActionResult<Direccion>> PostDireccion(Direccion direccion)
        {
            var createdDireccion = await _direccionService.CreateAsync(direccion);
            return CreatedAtAction(nameof(GetDireccion), new { id = createdDireccion.Id }, createdDireccion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDireccion(long id, Direccion direccion)
        {
            if (id != direccion.Id) return BadRequest();
            
            var updatedDireccion = await _direccionService.UpdateAsync(direccion);
            if (updatedDireccion == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDireccion(long id)
        {
            var deleted = await _direccionService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
