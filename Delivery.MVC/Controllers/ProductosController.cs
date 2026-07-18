using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly IProductoConsumer _consumer;
        private readonly IRestauranteConsumer _restauranteConsumer;
        private readonly ICategoriaProductoConsumer _categoriaConsumer;

        public ProductosController(IProductoConsumer consumer, IRestauranteConsumer restauranteConsumer, ICategoriaProductoConsumer categoriaConsumer)
        {
            _consumer = consumer;
            _restauranteConsumer = restauranteConsumer;
            _categoriaConsumer = categoriaConsumer;
        }

        private async Task CargarViewBags()
        {
            var restaurantes = await _restauranteConsumer.GetAllAsync();
            var categorias = await _categoriaConsumer.GetAllAsync();
            ViewBag.RestauranteId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(restaurantes, "Id", "Nombre");
            ViewBag.CategoriaId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categorias, "Id", "Nombre");
        }

        public async Task<IActionResult> Index()
        {
            var data = await _consumer.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            await CargarViewBags();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto entity)
        {
            // UML: Add_Menu_Item
            var prod = new Producto();
            prod.setName(entity.Nombre);
            prod.setUnitPrice(entity.Precio);
            
            // Assign back to entity for actual creation
            entity.Nombre = prod.Nombre;
            entity.Precio = prod.Precio;

            // Conceptual UML requirement: res.AddProduct(prod)
            var res = await _restauranteConsumer.GetByIdAsync(entity.RestauranteId);
            if (res != null) {
                res.AddProduct(prod);
            }

            await _consumer.CreateAsync(entity);
            TempData["Exito"] = "MenuItemAdded(): Producto agregado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            await CargarViewBags();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Producto entity)
        {
            await _consumer.UpdateAsync(id, entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _consumer.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
