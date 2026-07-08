using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantesController : ControllerBase
    {
        private readonly IRestauranteService _restauranteService;

        public RestaurantesController(IRestauranteService restauranteService)
        {
            _restauranteService = restauranteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurante>>> GetRestaurantes()
        {
            var restaurantes = await _restauranteService.GetAllAsync();
            return Ok(restaurantes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurante>> GetRestaurante(long id)
        {
            var restaurante = await _restauranteService.GetByIdAsync(id);
            if (restaurante == null) return NotFound();
            return Ok(restaurante);
        }

        [HttpPost]
        public async Task<ActionResult<Restaurante>> PostRestaurante(Restaurante restaurante)
        {
            var created = await _restauranteService.CreateAsync(restaurante);
            return CreatedAtAction(nameof(GetRestaurante), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurante(long id, Restaurante restaurante)
        {
            if (id != restaurante.Id) return BadRequest();
            
            var updated = await _restauranteService.UpdateAsync(restaurante);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurante(long id)
        {
            var deleted = await _restauranteService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
