using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteCuponesController : Controller
    {
        private readonly ICuponConsumer _cuponConsumer;
        private readonly ICuponUsuarioConsumer _cuponUsuarioConsumer;

        public ClienteCuponesController(ICuponConsumer cuponConsumer, ICuponUsuarioConsumer cuponUsuarioConsumer)
        {
            _cuponConsumer = cuponConsumer;
            _cuponUsuarioConsumer = cuponUsuarioConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);

            // Mostrar cupones activos globales
            var todosCupones = await _cuponConsumer.GetAllAsync();
            var cuponesActivos = todosCupones.Where(c => c.Activo && System.DateTime.UtcNow <= c.FechaFin).ToList();
            
            return View(cuponesActivos);
        }
    }
}
