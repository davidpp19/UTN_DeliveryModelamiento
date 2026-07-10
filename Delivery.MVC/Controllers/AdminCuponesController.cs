using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCuponesController : Controller
    {
        private readonly ICuponConsumer _cuponConsumer;

        public AdminCuponesController(ICuponConsumer cuponConsumer)
        {
            _cuponConsumer = cuponConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _cuponConsumer.GetAllAsync();
            return View(data);
        }
    }
}
