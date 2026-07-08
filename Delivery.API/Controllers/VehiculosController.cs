using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;

        public VehiculosController(IVehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos()
        {
            var vehiculos = await _vehiculoService.GetAllAsync();
            return Ok(vehiculos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculo(long id)
        {
            var vehiculo = await _vehiculoService.GetByIdAsync(id);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            var created = await _vehiculoService.CreateAsync(vehiculo);
            return CreatedAtAction(nameof(GetVehiculo), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculo(long id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id) return BadRequest();
            
            var updated = await _vehiculoService.UpdateAsync(vehiculo);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(long id)
        {
            var deleted = await _vehiculoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
