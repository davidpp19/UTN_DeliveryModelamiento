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
        private readonly ICategoriaProductoConsumer _categoriaConsumer;
        private readonly Delivery.MVC.Servicios.IArchivoService _archivoService;

        public RestauranteProductosController(IProductoConsumer productoConsumer, IRestauranteConsumer restauranteConsumer, ICategoriaProductoConsumer categoriaConsumer, Delivery.MVC.Servicios.IArchivoService archivoService)
        {
            _productoConsumer = productoConsumer;
            _restauranteConsumer = restauranteConsumer;
            _categoriaConsumer = categoriaConsumer;
            _archivoService = archivoService;
        }

        private async Task CargarCategoriasViewBagAsync(long restauranteId, long? seleccionId = null)
        {
            var categorias = await _categoriaConsumer.GetAllAsync();
            var misCategorias = categorias.Where(c => c.RestauranteId == restauranteId).ToList();
            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(misCategorias, "Id", "Nombre", seleccionId);
            ViewBag.TieneCategorias = misCategorias.Any();
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

            var restaurantes = await _restauranteConsumer.GetAllAsync();
            var miRestaurante = restaurantes.FirstOrDefault(r => r.Id == restauranteId);
            ViewBag.RestauranteNombre = miRestaurante?.Nombre ?? "Mi Restaurante";

            var categorias = await _categoriaConsumer.GetAllAsync();
            var categoria = categorias.FirstOrDefault(c => c.Id == data.CategoriaId);
            ViewBag.CategoriaNombre = categoria?.Nombre ?? "Sin Categoría";

            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return Unauthorized();
            
            await CargarCategoriasViewBagAsync(restauranteId.Value);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto entity, Microsoft.AspNetCore.Http.IFormFile? imagen)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                await CargarCategoriasViewBagAsync(restauranteId.Value, entity.CategoriaId);
                return View(entity);
            }

            if (imagen != null)
            {
                var url = await _archivoService.GuardarArchivoAsync(imagen, "productos");
                if (url != null) entity.ImagenUrl = url;
            }

            entity.RestauranteId = restauranteId.Value;
            await _productoConsumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _productoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            
            await CargarCategoriasViewBagAsync(restauranteId.Value, data.CategoriaId);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Producto entity, Microsoft.AspNetCore.Http.IFormFile? imagen)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null || entity.RestauranteId != restauranteId) return Unauthorized();

            if (!ModelState.IsValid)
            {
                await CargarCategoriasViewBagAsync(restauranteId.Value, entity.CategoriaId);
                return View(entity);
            }

            if (imagen != null)
            {
                var url = await _archivoService.GuardarArchivoAsync(imagen, "productos");
                if (url != null) entity.ImagenUrl = url;
            }

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
