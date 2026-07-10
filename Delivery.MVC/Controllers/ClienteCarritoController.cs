using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;
using Delivery.MVC.Helpers;

namespace Delivery.MVC.Controllers
{
    /// <summary>
    /// Carrito basado en Session del servidor.
    /// Flujo:
    ///   Agregar → modifica Session (NO toca base de datos)
    ///   Quitar  → modifica Session (NO toca base de datos)
    ///   Confirmar → llama API crear-desde-carrito (aquí sí se crea el Pedido)
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class ClienteCarritoController : Controller
    {
        private readonly IDireccionConsumer _direccionConsumer;
        private readonly IProductoConsumer _productoConsumer;
        private readonly IRestauranteConsumer _restauranteConsumer;
        private readonly IPedidoConsumer _pedidoConsumer;

        public ClienteCarritoController(
            IDireccionConsumer direccionConsumer,
            IProductoConsumer productoConsumer,
            IRestauranteConsumer restauranteConsumer,
            IPedidoConsumer pedidoConsumer)
        {
            _direccionConsumer = direccionConsumer;
            _productoConsumer = productoConsumer;
            _restauranteConsumer = restauranteConsumer;
            _pedidoConsumer = pedidoConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        // ─────────────────────────────────────────────────────────────────────
        // VER CARRITO
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session)
                          ?? new CarritoSesionDto();

            var userId = GetMyUsuarioId();
            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            ViewBag.SinDireccion = !misDirecciones.Any();
            ViewBag.Direcciones  = new SelectList(misDirecciones, "Id", "Calle");

            return View(carrito);
        }

        // ─────────────────────────────────────────────────────────────────────
        // AGREGAR PRODUCTO → SOLO modifica Session, NO toca la base de datos
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Agregar(long productoId, long restauranteId, int cantidad = 1)
        {
            // Verificar que el producto exista
            var producto = await _productoConsumer.GetByIdAsync(productoId);
            if (producto == null || !producto.Disponible)
            {
                TempData["Error"] = "El producto no está disponible.";
                return RedirectToAction("Restaurante", "Home", new { id = restauranteId });
            }

            // Verificar que el restaurante exista
            var restaurante = await _restauranteConsumer.GetByIdAsync(restauranteId);
            if (restaurante == null)
            {
                TempData["Error"] = "El restaurante no existe.";
                return RedirectToAction("Index", "Home");
            }

            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session)
                          ?? new CarritoSesionDto();

            // Si el carrito tiene ítems de otro restaurante, impedir mezcla
            if (carrito.RestauranteId != 0 && carrito.RestauranteId != restauranteId && carrito.Items.Any())
            {
                TempData["Error"] = "No puedes mezclar productos de diferentes restaurantes. Vacía el carrito primero.";
                return RedirectToAction("Index");
            }

            carrito.RestauranteId     = restauranteId;
            carrito.NombreRestaurante = restaurante.Nombre;

            // Si el producto ya está en el carrito, incrementar cantidad
            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItemSesionDto
                {
                    ProductoId      = productoId,
                    NombreProducto  = producto.Nombre,
                    Cantidad        = cantidad,
                    PrecioUnitario  = producto.Precio
                });
            }

            // Guardar en Session → NADA en base de datos
            CarritoSessionHelper.GuardarCarrito(HttpContext.Session, carrito);
            TempData["Exito"] = $"'{producto.Nombre}' agregado al carrito.";

            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────────────────
        // QUITAR PRODUCTO → SOLO modifica Session
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public IActionResult Quitar(long productoId)
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session);
            if (carrito != null)
            {
                carrito.Items.RemoveAll(i => i.ProductoId == productoId);
                if (!carrito.Items.Any())
                {
                    // Carrito vacío → limpiar restaurante también
                    carrito = new CarritoSesionDto();
                }
                CarritoSessionHelper.GuardarCarrito(HttpContext.Session, carrito);
            }
            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────────────────
        // CHECKOUT - VISTA DE PAGO
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Checkout()
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session);
            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            var userId = GetMyUsuarioId();
            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            if (!misDirecciones.Any())
            {
                TempData["Error"] = "Necesitas registrar una dirección antes de confirmar tu pedido.";
                return RedirectToAction("Index", "ClienteDirecciones");
            }

            ViewBag.Direcciones = new SelectList(misDirecciones, "Id", "Calle");
            return View(carrito);
        }

        // ─────────────────────────────────────────────────────────────────────
        // CONFIRMAR COMPRA → Aquí se crea el Pedido real en base de datos
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Confirmar(long direccionId, string metodoPago, string? titular, string? numeroTarjeta, string? expiracion, string? cvv)
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session);
            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            if (direccionId <= 0)
            {
                TempData["Error"] = "Debes seleccionar una dirección de entrega válida.";
                return RedirectToAction("Checkout");
            }

            // --- SIMULACIÓN DE PAGO ---
            var tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Efectivo;

            if (metodoPago == "Tarjeta")
            {
                tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Tarjeta;
                // Validación básica de tarjeta
                if (string.IsNullOrWhiteSpace(numeroTarjeta) || string.IsNullOrWhiteSpace(expiracion) || string.IsNullOrWhiteSpace(cvv))
                {
                    TempData["Error"] = "Todos los campos de la tarjeta son obligatorios.";
                    return RedirectToAction("Checkout");
                }

                numeroTarjeta = numeroTarjeta.Replace(" ", "").Replace("-", "");
                if (!EsTarjetaValidaLuhn(numeroTarjeta))
                {
                    TempData["Error"] = "El número de tarjeta no es válido (Validación Luhn fallida).";
                    return RedirectToAction("Checkout");
                }
                
                string marca = ObtenerMarcaTarjeta(numeroTarjeta);
                if (marca == "Desconocida")
                {
                    TempData["Error"] = "La tarjeta no es Visa, Mastercard, Amex ni Discover.";
                    return RedirectToAction("Checkout");
                }

                TempData["MensajePago"] = $"Pago aprobado (simulación) con tarjeta {marca}.";
            }
            else if (metodoPago == "Transferencia")
            {
                tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Transferencia;
                TempData["MensajePago"] = "Pago por transferencia verificado (simulación).";
            }
            else
            {
                tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Efectivo;
            }

            var userId = GetMyUsuarioId();

            // *** ÚNICO PUNTO donde se crea el Pedido en base de datos ***
            var pedidoCreado = await _pedidoConsumer.CrearDesdeCarritoAsync(userId, direccionId, tipoMetodoPago, carrito);

            if (pedidoCreado != null)
            {
                CarritoSessionHelper.LimpiarCarrito(HttpContext.Session);
                TempData["Exito"] = $"¡Pedido #{pedidoCreado.Id} confirmado exitosamente! {TempData["MensajePago"]}";
                return RedirectToAction("Index", "ClienteHistorialPedidos");
            }

            TempData["Error"] = "Hubo un problema al procesar tu pedido. Por favor intenta de nuevo.";
            return RedirectToAction("Checkout");
        }

        private bool EsTarjetaValidaLuhn(string numero)
        {
            int sum = 0;
            bool alt = false;
            for (int i = numero.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(numero[i])) return false;
                int n = numero[i] - '0';
                if (alt)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alt = !alt;
            }
            return (sum % 10 == 0);
        }

        private string ObtenerMarcaTarjeta(string numero)
        {
            if (numero.StartsWith("4")) return "Visa";
            if (numero.StartsWith("51") || numero.StartsWith("52") || numero.StartsWith("53") || numero.StartsWith("54") || numero.StartsWith("55")) return "Mastercard";
            if (numero.StartsWith("34") || numero.StartsWith("37")) return "American Express";
            if (numero.StartsWith("6011") || numero.StartsWith("65")) return "Discover";
            return "Desconocida";
        }
    }
}
