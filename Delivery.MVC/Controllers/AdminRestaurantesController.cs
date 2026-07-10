using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRestaurantesController : Controller
    {
        private readonly IRestauranteConsumer _restauranteConsumer;

        public AdminRestaurantesController(IRestauranteConsumer restauranteConsumer)
        {
            _restauranteConsumer = restauranteConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _restauranteConsumer.GetAllAsync();
            return View(data);
        }
    }
}
