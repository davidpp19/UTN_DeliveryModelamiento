using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Repartidor")]
    public class RepartidorIncidentesController : Controller
    {
        // UML: Incident Report Form
        [HttpGet]
        public IActionResult Reportar()
        {
            return View();
        }

        // UML: Validate Report Data -> Incident Table -> Notify Admin -> Update System Log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reportar(string description, string severity)
        {
            if (string.IsNullOrEmpty(description))
            {
                ModelState.AddModelError(string.Empty, "La descripción del incidente es obligatoria.");
                return View();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Simulation of "Incident Table" and "Notify Admin"
            // We'll log it directly via TempData for now
            TempData["Exito"] = $"Incidente reportado exitosamente. Nivel: {severity}. (UML: Notify Admin / Update System Log)";
            
            // UML: Ok and Return
            return RedirectToAction("Index", "DashboardRepartidor");
        }
    }
}
