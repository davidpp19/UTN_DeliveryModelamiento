using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Restaurante")]
    public class DashboardRestauranteController : Controller
    {
        private readonly IRestauranteConsumer _restauranteConsumer;

        public DashboardRestauranteController(IRestauranteConsumer restauranteConsumer)
        {
            _restauranteConsumer = restauranteConsumer;
        }

        public async System.Threading.Tasks.Task<IActionResult> Index([FromServices] IUsuarioConsumer usuarioConsumer)
        {
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var notificaciones = await usuarioConsumer.GetNotificacionesAsync(userId);
                ViewBag.Notificaciones = notificaciones.OrderByDescending(n => n.Id).Take(5).ToList();

                var restaurantes = await _restauranteConsumer.GetAllAsync();
                var miRestaurante = restaurantes.FirstOrDefault(r => r.CreadoPor == userId);
                if (miRestaurante != null)
                {
                    ViewBag.Estado = miRestaurante.Estado.ToString();
                    if (miRestaurante.Estado == Delivery.Modelos.Enums.EstadoRestauranteEnum.Pendiente ||
                        miRestaurante.Estado == Delivery.Modelos.Enums.EstadoRestauranteEnum.Rechazado)
                    {
                        return View();
                    }
                }
            }
            return View();
        }
    }
}
