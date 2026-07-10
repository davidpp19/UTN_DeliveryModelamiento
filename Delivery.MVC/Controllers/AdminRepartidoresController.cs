using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRepartidoresController : Controller
    {
        private readonly IRepartidorConsumer _repartidorConsumer;

        public AdminRepartidoresController(IRepartidorConsumer repartidorConsumer)
        {
            _repartidorConsumer = repartidorConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repartidorConsumer.GetAllAsync();
            return View(data);
        }
    }
}
