using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly IFavoritoService _favoritoService;

        public FavoritosController(IFavoritoService favoritoService)
        {
            _favoritoService = favoritoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorito>>> GetFavoritos()
        {
            var favoritos = await _favoritoService.GetAllAsync();
            return Ok(favoritos);
        }

        [HttpGet("{usuarioId}/{restauranteId}")]
        public async Task<ActionResult<Favorito>> GetFavorito(long usuarioId, long restauranteId)
        {
            var favorito = await _favoritoService.GetByIdsAsync(usuarioId, restauranteId);
            if (favorito == null) return NotFound();
            return Ok(favorito);
        }

        [HttpPost]
        public async Task<ActionResult<Favorito>> PostFavorito(Favorito favorito)
        {
            var created = await _favoritoService.CreateAsync(favorito);
            return CreatedAtAction(nameof(GetFavorito), new { usuarioId = created.UsuarioId, restauranteId = created.RestauranteId }, created);
        }

        [HttpDelete("{usuarioId}/{restauranteId}")]
        public async Task<IActionResult> DeleteFavorito(long usuarioId, long restauranteId)
        {
            var deleted = await _favoritoService.DeleteAsync(usuarioId, restauranteId);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
