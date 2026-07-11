using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Restaurante,Admin")]
    public class RestaurantePedidosController : Controller
    {
        private readonly IPedidoConsumer _pedidoConsumer;
        private readonly IRestauranteConsumer _restauranteConsumer;

        public RestaurantePedidosController(IPedidoConsumer pedidoConsumer, IRestauranteConsumer restauranteConsumer)
        {
            _pedidoConsumer = pedidoConsumer;
            _restauranteConsumer = restauranteConsumer;
        }

        private async Task<long?> GetMyRestauranteId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var restaurantes = await _restauranteConsumer.GetAllAsync();
                var miRestaurante = restaurantes.FirstOrDefault(r => r.CreadoPor == userId);
                return miRestaurante?.Id;
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return View("SinRestaurante");

            var todos = await _pedidoConsumer.GetAllAsync();
            var misPedidos = todos.Where(p => p.RestauranteId == restauranteId.Value).OrderByDescending(p => p.FechaPedido);
            return View(misPedidos);
        }

        public async Task<IActionResult> Details(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();
            return View(data);
        }

        public async Task<IActionResult> CambiarEstado(long id)
        {
            var restauranteId = await GetMyRestauranteId();
            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId) return NotFound();

            var estadosPermitidos = new System.Collections.Generic.List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            switch (data.EstadoPedido)
            {
                case EstadoPedidoEnum.Pendiente:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Aceptado", Value = "Aceptado" });
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Cancelado", Value = "Cancelado" });
                    break;
                case EstadoPedidoEnum.Aceptado:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "En Preparación", Value = "EnPreparacion" });
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Cancelado", Value = "Cancelado" });
                    break;
                case EstadoPedidoEnum.EnPreparacion:
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Listo para Recoger", Value = "ListoParaRecoger" });
                    estadosPermitidos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Cancelado", Value = "Cancelado" });
                    break;
            }
            ViewBag.EstadosPermitidos = estadosPermitidos;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(long id, EstadoPedidoEnum nuevoEstado)
        {
            var restauranteId = await GetMyRestauranteId();
            if (restauranteId == null) return Unauthorized();

            var data = await _pedidoConsumer.GetByIdAsync(id);
            if (data == null || data.RestauranteId != restauranteId.Value) return NotFound();

            var result = await _pedidoConsumer.ActualizarEstadoRestauranteAsync(id, nuevoEstado, restauranteId.Value);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error al actualizar el estado del pedido.");
            return View(data);
        }
    }
}
