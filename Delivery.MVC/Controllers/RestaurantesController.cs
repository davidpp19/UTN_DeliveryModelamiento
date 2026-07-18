using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RestaurantesController : Controller
    {
        private readonly IRestauranteConsumer _consumer;

        public RestaurantesController(IRestauranteConsumer consumer)
        {
            _consumer = consumer;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Restaurante entity)
        {
            // UML: Register_Restaurant
            if (entity.ValidLicense)
            {
                // Sequence diagram requires setName and setAddress
                var res = new Restaurante();
                res.setName(entity.Nombre);
                res.setAddress(entity.Calle);
                
                // Copy the rest of the properties for real logic
                entity.Nombre = res.Nombre;
                entity.Calle = res.Calle;

                await _consumer.CreateAsync(entity);
                TempData["Exito"] = "RestaurantRegistered()";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // RegistrationRejected(reason)
                TempData["Error"] = "RegistrationRejected(Invalid License)";
                return View(entity);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Restaurante entity)
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
