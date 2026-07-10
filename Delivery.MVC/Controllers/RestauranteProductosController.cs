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
    public class RestauranteProductosController : Controller
    {
        private readonly IProductoConsumer _productoConsumer;
        private readonly IRestauranteConsumer _restauranteConsumer;

        public RestauranteProductosController(IProductoConsumer productoConsumer, IRestauranteConsumer restauranteConsumer)
        {
            _productoConsumer = productoConsumer;
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
            var miRestaurante = restaurantes.FirstOrDefault(r => r.Id == restauranteId.Value);
            if (miRestaurante != null && (miRestaurante.Estado == Delivery.Modelos.Enums.EstadoRestauranteEnum.Pendiente ||
                                          miRestaurante.Estado == Delivery.Modelos.Enums.EstadoRestauranteEnum.Rechazado))
            {
                return RedirectToAction("Index", "DashboardRestaurante");
            }

            var todos = await _productoConsumer.GetAllAsync();
            var misProductos = todos.Where(p => p.RestauranteId == restauranteId.Value);
            return View(misProductos);
        }

        public async Task<IActionResult> Details(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _productoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto entity)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return Unauthorized();

            entity.RestauranteId = restauranteId.Value;
            await _productoConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _productoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Producto entity)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null || entity.RestauranteId != restauranteId) return Unauthorized();

            await _productoConsumer.UpdateAsync(id, entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _productoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _productoConsumer.GetByIdAsync(id);
            if (data != null && data.RestauranteId == restauranteId)
            {
                await _productoConsumer.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
