using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Restaurante,Admin")]
    public class RestauranteCategoriasController : Controller
    {
        private readonly ICategoriaProductoConsumer _categoriaConsumer;
        private readonly IRestauranteConsumer _restauranteConsumer;

        public RestauranteCategoriasController(ICategoriaProductoConsumer categoriaConsumer, IRestauranteConsumer restauranteConsumer)
        {
            _categoriaConsumer = categoriaConsumer;
            _restauranteConsumer = restauranteConsumer;
        }

        private async Task<long?> GetMyRestauranteId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var restaurantes = await _restauranteConsumer.GetAllAsync();
                var miRestaurante = restaurantes.FirstOrDefault(r => r.CreadoPor == userId);
                return miRestaurante?.Id;
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return View("SinRestaurante");

            var restaurantes = await _restauranteConsumer.GetAllAsync();
            var miRestaurante = restaurantes.FirstOrDefault(r => r.Id == restauranteId);
            ViewBag.RestauranteNombre = miRestaurante?.Nombre ?? "Mi Restaurante";

            var todos = await _categoriaConsumer.GetAllAsync();
            var misCategorias = todos.Where(c => c.RestauranteId == restauranteId.Value);
            return View(misCategorias);
        }

        public async Task<IActionResult> Details(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _categoriaConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();

            var restaurantes = await _restauranteConsumer.GetAllAsync();
            var miRestaurante = restaurantes.FirstOrDefault(r => r.Id == restauranteId);
            ViewBag.RestauranteNombre = miRestaurante?.Nombre ?? "Mi Restaurante";

            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoriaProducto entity)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return Unauthorized();

            entity.RestauranteId = restauranteId.Value;
            await _categoriaConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _categoriaConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, CategoriaProducto entity)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null || entity.RestauranteId != restauranteId) return Unauthorized();

            await _categoriaConsumer.UpdateAsync(id, entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _categoriaConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _categoriaConsumer.GetByIdAsync(id);
            if (data != null && data.RestauranteId == restauranteId)
            {
                await _categoriaConsumer.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
