using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;

        public AdminPedidosController(IPedidoConsumer pedidoConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _pedidoConsumer.GetAllAsync();
            return View(data);
        }
    }
}
