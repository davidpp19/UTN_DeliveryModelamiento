using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Repartidor")]
    public class DashboardRepartidorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
