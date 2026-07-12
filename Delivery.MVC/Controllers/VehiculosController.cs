using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehiculosController : Controller
    {
        private readonly IVehiculoConsumer _consumer;
        private readonly IRepartidorConsumer _repartidorConsumer;

        public VehiculosController(IVehiculoConsumer consumer, IRepartidorConsumer repartidorConsumer)
        {
            _consumer = consumer;
            _repartidorConsumer = repartidorConsumer;
        }

        private async Task CargarViewBags()
        {
            var repartidores = await _repartidorConsumer.GetAllAsync();
            // Assuming Repartidor has a Usuario navigation property, we might just show ID for now, 
            // but ideally we'd join with Usuario. For now, since it's a simple CRUD, just using Id is better than raw input box.
            // Wait, we need a string. Let's just use Id since we don't have an easy Name property directly on Repartidor without fetching Usuario.
            ViewBag.RepartidorId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(repartidores, "Id", "Id");
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
        public async Task<IActionResult> Create(Vehiculo entity)
        {
            await _consumer.CreateAsync(entity);
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
        public async Task<IActionResult> Edit(long id, Vehiculo entity)
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
