using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioConsumer _consumer;
        private readonly IRolConsumer _rolConsumer;

        public UsuariosController(IUsuarioConsumer consumer, IRolConsumer rolConsumer)
        {
            _consumer = consumer;
            _rolConsumer = rolConsumer;
        }

        private async Task CargarViewBags()
        {
            var roles = await _rolConsumer.GetAllAsync();
            ViewBag.RolId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roles, "Id", "Nombre");
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
        public async Task<IActionResult> Create(Usuario entity)
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
        public async Task<IActionResult> Edit(long id, Usuario entity)
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
