using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsuariosController : Controller
    {
        private readonly IUsuarioConsumer _usuarioConsumer;

        public AdminUsuariosController(IUsuarioConsumer usuarioConsumer)
        {
            _usuarioConsumer = usuarioConsumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _usuarioConsumer.GetAllAsync();
            return View(data);
        }
    }
}
