using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Restaurante")]
    public class DashboardRestauranteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
