using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
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
        private readonly Delivery.MVC.Servicios.IArchivoService _archivoService;

        public ClienteCarritoController(
            IDireccionConsumer direccionConsumer,
            IProductoConsumer productoConsumer,
            IRestauranteConsumer restauranteConsumer,
            IPedidoConsumer pedidoConsumer,
            Delivery.MVC.Servicios.IArchivoService archivoService)
        {
            _direccionConsumer = direccionConsumer;
            _productoConsumer = productoConsumer;
            _restauranteConsumer = restauranteConsumer;
            _pedidoConsumer = pedidoConsumer;
            _archivoService = archivoService;
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
                    ImagenUrl       = producto.ImagenUrl,
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

        [HttpGet]
        public async Task<IActionResult> CalcularCostoEnvio(long direccionId, double? lat, double? lon)
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session);
            if (carrito == null || carrito.RestauranteId == 0) return Json(new { exito = false });

            var restaurante = await _restauranteConsumer.GetByIdAsync(carrito.RestauranteId);
            
            double? dLat = lat;
            double? dLon = lon;

            if (direccionId > 0)
            {
                var direccion = await _direccionConsumer.GetByIdAsync(direccionId);
                if (direccion != null)
                {
                    dLat = (double?)direccion.Latitud;
                    dLon = (double?)direccion.Longitud;
                }
            }

            if (restaurante == null || !restaurante.Latitud.HasValue || !restaurante.Longitud.HasValue || !dLat.HasValue || !dLon.HasValue)
            {
                return Json(new { exito = true, costo = restaurante?.CostoEnvioBase ?? 1.50m, distancia = 0, tiempo = 0 });
            }

            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "RayoExpresDeliveryApp/1.0");
                
                var rLat = restaurante.Latitud.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var rLon = restaurante.Longitud.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var cLat = dLat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var cLon = dLon.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                var url = $"http://router.project-osrm.org/route/v1/driving/{rLon},{rLat};{cLon},{cLat}?overview=false";
                var response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    
                    var code = doc.RootElement.GetProperty("code").GetString();
                    if (code == "Ok")
                    {
                        var routes = doc.RootElement.GetProperty("routes");
                        if (routes.GetArrayLength() > 0)
                        {
                            var route = routes[0];
                            var distanceMeters = route.GetProperty("distance").GetDouble();
                            var durationSeconds = route.GetProperty("duration").GetDouble();
                            
                            var distKm = distanceMeters / 1000.0;
                            var tiempoMin = durationSeconds / 60.0;
                            
                            var distanciaRedondeada = System.Math.Ceiling(distKm);
                            decimal costoFinal = 1.50m;
                            if (distanciaRedondeada > 3)
                            {
                                costoFinal += (decimal)(distanciaRedondeada - 3) * 0.25m;
                            }
                            
                            return Json(new { exito = true, costo = costoFinal, distancia = distKm, tiempo = tiempoMin });
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Fallback to Haversine
            }

            // Fallback (Haversine)
            var r = 6371;
            var radLat = (dLat.Value - (double)restaurante.Latitud.Value) * System.Math.PI / 180.0;
            var radLon = (dLon.Value - (double)restaurante.Longitud.Value) * System.Math.PI / 180.0;
            var a = System.Math.Sin(radLat / 2) * System.Math.Sin(radLat / 2) +
                    System.Math.Cos((double)restaurante.Latitud.Value * System.Math.PI / 180.0) * System.Math.Cos(dLat.Value * System.Math.PI / 180.0) *
                    System.Math.Sin(radLon / 2) * System.Math.Sin(radLon / 2);
            var c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
            var haversineDist = r * c;

            var distRedondeada = System.Math.Ceiling(haversineDist);
            decimal fallbackCosto = 1.50m;
            if (distRedondeada > 3)
            {
                fallbackCosto += (decimal)(distRedondeada - 3) * 0.25m;
            }

            return Json(new { exito = true, costo = fallbackCosto, distancia = haversineDist, tiempo = haversineDist * 2 });
        }

        // ─────────────────────────────────────────────────────────────────────
        // CONFIRMAR COMPRA → Aquí se crea el Pedido real en base de datos
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Confirmar(
            long direccionId, 
            string metodoPago, 
            string? titular, 
            string? numeroTarjeta, 
            string? expiracion, 
            string? cvv, 
            Microsoft.AspNetCore.Http.IFormFile? comprobante,
            string? nuevaCalle,
            string? nuevaCiudad,
            double? nuevaLatitud,
            double? nuevaLongitud,
            string? nuevoAlias,
            string? nuevaReferencia,
            bool guardarDireccion)
        {
            var carrito = CarritoSessionHelper.ObtenerCarrito(HttpContext.Session);
            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            var userId = GetMyUsuarioId();
            long finalDireccionId = direccionId;

            // Lógica para crear nueva dirección si se eligió o arrastró en el mapa (direccionId = 0)
            if (direccionId == 0)
            {
                if (string.IsNullOrWhiteSpace(nuevaCalle) || !nuevaLatitud.HasValue || !nuevaLongitud.HasValue)
                {
                    TempData["Error"] = "Debes proporcionar una ubicación válida en el mapa.";
                    return RedirectToAction("Checkout");
                }

                var direccion = new Direccion
                {
                    UsuarioId = userId,
                    Calle = nuevaCalle,
                    Ciudad = nuevaCiudad ?? "No especificada",
                    Latitud = (decimal?)nuevaLatitud,
                    Longitud = (decimal?)nuevaLongitud,
                    Alias = guardarDireccion ? (string.IsNullOrWhiteSpace(nuevoAlias) ? "Mi nueva dirección" : nuevoAlias) : "Dirección de Pedido",
                    Referencia = nuevaReferencia,
                    EsPrincipal = false,
                    CreadoEn = System.DateTime.UtcNow
                };

                var createdDir = await _direccionConsumer.CreateAsync(direccion);
                if (createdDir == null || createdDir.Id == 0)
                {
                    TempData["Error"] = "No se pudo guardar la ubicación en el sistema.";
                    return RedirectToAction("Checkout");
                }
                
                finalDireccionId = createdDir.Id;
            }
            else if (finalDireccionId <= 0)
            {
                TempData["Error"] = "Debes seleccionar una dirección de entrega válida.";
                return RedirectToAction("Checkout");
            }

            // --- SIMULACIÓN DE PAGO ---
            var tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Efectivo;
            string? comprobanteUrl = null;

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
                if (comprobante != null && comprobante.Length > 0)
                {
                    comprobanteUrl = await _archivoService.GuardarArchivoAsync(comprobante, "comprobantes");
                }
                TempData["MensajePago"] = "Pago por transferencia verificado (simulación).";
            }
            else
            {
                tipoMetodoPago = Delivery.Modelos.Enums.TipoMetodoPagoEnum.Efectivo;
            }

            // Guardamos temporalmente el comprobante
            if (!string.IsNullOrEmpty(comprobanteUrl))
            {
                carrito.ComprobanteTransferenciaUrl = comprobanteUrl;
            }

            // *** ÚNICO PUNTO donde se crea el Pedido en base de datos ***
            var pedidoCreado = await _pedidoConsumer.CrearDesdeCarritoAsync(userId, finalDireccionId, tipoMetodoPago, carrito);

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
