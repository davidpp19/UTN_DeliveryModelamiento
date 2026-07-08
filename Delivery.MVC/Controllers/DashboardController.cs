using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardConsumer _dashboardConsumer;

        public DashboardController(IDashboardConsumer dashboardConsumer)
        {
            _dashboardConsumer = dashboardConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var estadisticas = await _dashboardConsumer.ObtenerEstadisticasAsync();
            return View(estadisticas);
        }
    }
}
