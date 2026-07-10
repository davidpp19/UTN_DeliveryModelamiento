using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductosController : Controller
    {
        private readonly IProductoConsumer _productoConsumer;

        public AdminProductosController(IProductoConsumer productoConsumer)
        {
            _productoConsumer = productoConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _productoConsumer.GetAllAsync();
            return View(data);
        }
    }
}
