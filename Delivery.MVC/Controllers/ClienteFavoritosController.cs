using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Cliente")]
    [Authorize(Roles = "Admin")]
    public class ClienteFavoritosController : Controller
    {
        private readonly IFavoritoConsumer _favoritoConsumer;
        private readonly IRestauranteConsumer _restauranteConsumer;

        public ClienteFavoritosController(IFavoritoConsumer favoritoConsumer, IRestauranteConsumer restauranteConsumer)
        {
            _favoritoConsumer = favoritoConsumer;
            _restauranteConsumer = restauranteConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var todos = await _favoritoConsumer.GetAllAsync();
            var misFavoritos = todos.Where(f => f.UsuarioId == userId);
            return View(misFavoritos);
        }

        public async Task<IActionResult> Create()
        {
            var restaurantes = await _restauranteConsumer.GetAllAsync();
            ViewBag.Restaurantes = new SelectList(restaurantes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Favorito entity)
        {
            var userId = GetMyUsuarioId();
            entity.UsuarioId = userId;
            await _favoritoConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id) // id is restauranteId
        {
            var userId = GetMyUsuarioId();
            var data = await _favoritoConsumer.GetByIdsAsync(userId, id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userId = GetMyUsuarioId();
            await _favoritoConsumer.DeleteAsync(userId, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
