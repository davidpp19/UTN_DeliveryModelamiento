using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            
            var direcciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = direcciones.Where(d => d.UsuarioId == userId).ToList();
            ViewBag.Direcciones = new SelectList(misDirecciones, "Id", "Calle");

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
            var pedidoConfirmado = await _carritoConsumer.ConfirmarCarritoAsync(userId, direccionId);
            
            if (pedidoConfirmado != null)
            {
                return RedirectToAction("Index", "ClienteHistorialPedidos");
            }
            
            ModelState.AddModelError(string.Empty, "Hubo un problema al confirmar el carrito.");
            return RedirectToAction("Index");
        }
    }
}
