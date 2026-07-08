using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponesController : ControllerBase
    {
        private readonly ICuponService _cuponService;

        public CuponesController(ICuponService cuponService)
        {
            _cuponService = cuponService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupon>>> GetCupones()
        {
            var cupones = await _cuponService.GetAllAsync();
            return Ok(cupones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cupon>> GetCupon(long id)
        {
            var cupon = await _cuponService.GetByIdAsync(id);
            if (cupon == null) return NotFound();
            return Ok(cupon);
        }

        [HttpPost]
        public async Task<ActionResult<Cupon>> PostCupon(Cupon cupon)
        {
            var created = await _cuponService.CreateAsync(cupon);
            return CreatedAtAction(nameof(GetCupon), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupon(long id, Cupon cupon)
        {
            if (id != cupon.Id) return BadRequest();
            
            var updated = await _cuponService.UpdateAsync(cupon);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupon(long id)
        {
            var deleted = await _cuponService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
