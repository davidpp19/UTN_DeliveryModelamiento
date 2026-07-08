using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class AuditoriasController : Controller
    {
        private readonly IAuditoriaConsumer _consumer;

        public AuditoriasController(IAuditoriaConsumer consumer)
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
    }
}
