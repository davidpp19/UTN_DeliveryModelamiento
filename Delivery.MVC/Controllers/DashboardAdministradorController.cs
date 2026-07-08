using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardAdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
