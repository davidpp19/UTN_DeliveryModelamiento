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

            // Obtener los cupones que el usuario ha registrado
            var todosCuponesUsuarios = await _cuponUsuarioConsumer.GetAllAsync();
            var misCupones = todosCuponesUsuarios.Where(cu => cu.UsuarioId == userId).ToList();
            
            return View(misCupones);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["Error"] = "Debes ingresar un código.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);

            // 1. Validar que el cupón exista
            var todosCupones = await _cuponConsumer.GetAllAsync();
            var cupon = todosCupones.FirstOrDefault(c => c.Codigo.Equals(codigo.Trim(), System.StringComparison.OrdinalIgnoreCase));

            if (cupon == null)
            {
                TempData["Error"] = "El cupón no existe.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Validar que esté activo
            if (!cupon.Activo)
            {
                TempData["Error"] = "El cupón todavía no está activo.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Validar que no haya expirado
            if (System.DateTime.UtcNow > cupon.FechaFin)
            {
                TempData["Error"] = "El cupón expiró.";
                return RedirectToAction(nameof(Index));
            }

            // 4. Validar exclusividad
            if (!cupon.EsPublico && cupon.UsuarioExclusivoId != userId)
            {
                TempData["Error"] = "Este cupón no pertenece al usuario.";
                return RedirectToAction(nameof(Index));
            }

            // 5. Validar si ya lo registró
            var misCupones = await _cuponUsuarioConsumer.GetAllAsync();
            var yaRegistrado = misCupones.Any(cu => cu.UsuarioId == userId && cu.CuponId == cupon.Id);
            
            if (yaRegistrado)
            {
                TempData["Error"] = "El cupón ya fue registrado anteriormente.";
                return RedirectToAction(nameof(Index));
            }

            // Registrar cupón
            var nuevoRegistro = new Delivery.Modelos.Entidades.CuponUsuario
            {
                CuponId = cupon.Id,
                UsuarioId = userId,
                FechaRegistro = System.DateTime.UtcNow,
                PedidoId = null,
                FechaUso = null
            };

            await _cuponUsuarioConsumer.CreateAsync(nuevoRegistro);
            TempData["Success"] = "¡Cupón registrado correctamente!";

            return RedirectToAction(nameof(Index));
        }
    }
}
