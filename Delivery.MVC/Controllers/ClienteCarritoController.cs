using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteCarritoController : Controller
    {
        private readonly ICarritoConsumer _carritoConsumer;
        private readonly IDireccionConsumer _direccionConsumer;

        public ClienteCarritoController(ICarritoConsumer carritoConsumer, IDireccionConsumer direccionConsumer)
        {
            _carritoConsumer = carritoConsumer;
            _direccionConsumer = direccionConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var carrito = await _carritoConsumer.GetCarritoAsync(userId);

            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            // Si no tiene dirección, mostramos un aviso en la vista
            if (!misDirecciones.Any())
            {
                ViewBag.SinDireccion = true;
            }
            else
            {
                ViewBag.SinDireccion = false;
                ViewBag.Direcciones = new SelectList(misDirecciones, "Id", "Calle");
            }

            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(AgregarAlCarritoDto dto)
        {
            dto.UsuarioId = GetMyUsuarioId();
            await _carritoConsumer.AgregarProductoAsync(dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Quitar(long detallePedidoId)
        {
            var userId = GetMyUsuarioId();
            await _carritoConsumer.QuitarProductoAsync(userId, detallePedidoId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(long direccionId)
        {
            var userId = GetMyUsuarioId();

            // Validar que la dirección sea real y pertenezca al usuario
            if (direccionId <= 0)
            {
                TempData["Error"] = "Debes seleccionar una dirección de entrega válida.";
                return RedirectToAction("Index");
            }

            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            if (!misDirecciones.Any())
            {
                TempData["Error"] = "Necesitas registrar una dirección de entrega antes de confirmar tu pedido.";
                return RedirectToAction("Index", "ClienteDirecciones");
            }

            var direccionValida = misDirecciones.Any(d => d.Id == direccionId);
            if (!direccionValida)
            {
                TempData["Error"] = "La dirección seleccionada no es válida.";
                return RedirectToAction("Index");
            }

            var pedidoConfirmado = await _carritoConsumer.ConfirmarCarritoAsync(userId, direccionId);

            if (pedidoConfirmado != null)
            {
                TempData["Exito"] = "¡Tu pedido fue confirmado exitosamente!";
                return RedirectToAction("Index", "ClienteHistorialPedidos");
            }

            TempData["Error"] = "Hubo un problema al confirmar el carrito. Verifica que tengas productos agregados.";
            return RedirectToAction("Index");
        }
    }
}
