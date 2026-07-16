using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;
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
        private readonly ICuponConsumer _cuponConsumer;
        private readonly ICuponUsuarioConsumer _cuponUsuarioConsumer;
        private readonly ICarritoConsumer _carritoConsumer;

        public ClienteCarritoController(
            IDireccionConsumer direccionConsumer,
            IProductoConsumer productoConsumer,
            IRestauranteConsumer restauranteConsumer,
            IPedidoConsumer pedidoConsumer,
            Delivery.MVC.Servicios.IArchivoService archivoService,
            ICuponConsumer cuponConsumer,
            ICuponUsuarioConsumer cuponUsuarioConsumer,
            ICarritoConsumer carritoConsumer)
        {
            _direccionConsumer = direccionConsumer;
            _productoConsumer = productoConsumer;
            _restauranteConsumer = restauranteConsumer;
            _pedidoConsumer = pedidoConsumer;
            _archivoService = archivoService;
            _cuponConsumer = cuponConsumer;
            _cuponUsuarioConsumer = cuponUsuarioConsumer;
            _carritoConsumer = carritoConsumer;
        }

        private long GetMyUsuarioId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);
            return userId;
        }

        private async Task<CarritoSesionDto> ObtenerCarritoDtoAsync(long userId)
        {
            var carritoBd = await _carritoConsumer.GetCarritoAsync(userId);
            if (carritoBd == null) return new CarritoSesionDto();

            var dto = new CarritoSesionDto
            {
                RestauranteId = carritoBd.RestauranteId
            };
            
            var restaurante = await _restauranteConsumer.GetByIdAsync(carritoBd.RestauranteId);
            if (restaurante != null) dto.NombreRestaurante = restaurante.Nombre;

            if (carritoBd.Items != null)
            {
                foreach (var d in carritoBd.Items)
                {
                    dto.Items.Add(new CarritoItemSesionDto
                    {
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto?.Nombre ?? "Producto",
                        ImagenUrl = d.Producto?.ImagenUrl ?? "",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    });
                }
            }
            return dto;
        }

        // ─────────────────────────────────────────────────────────────────────
        // VER CARRITO
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var userId = GetMyUsuarioId();
            var carrito = await ObtenerCarritoDtoAsync(userId);
            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            ViewBag.SinDireccion = !misDirecciones.Any();
            ViewBag.Direcciones  = new SelectList(misDirecciones, "Id", "Calle");

            return View(carrito);
        }

        // ─────────────────────────────────────────────────────────────────────
        // AGREGAR PRODUCTO → Utiliza la base de datos a través de ICarritoConsumer
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Agregar(long productoId, long restauranteId, int cantidad = 1)
        {
            var userId = GetMyUsuarioId();
            
            var dto = new AgregarAlCarritoDto
            {
                UsuarioId = userId,
                ProductoId = productoId,
                RestauranteId = restauranteId,
                Cantidad = cantidad
            };

            try
            {
                var resultado = await _carritoConsumer.AgregarProductoAsync(dto);
                if (resultado == null)
                {
                    TempData["Error"] = "No se pudo agregar el producto. Puede que debas vaciar el carrito si es de otro restaurante.";
                }
                else
                {
                    TempData["Exito"] = "Producto agregado al carrito.";
                }
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Error al agregar: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────────────────
        // QUITAR PRODUCTO
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Quitar(long productoId)
        {
            var userId = GetMyUsuarioId();
            var carritoBd = await _carritoConsumer.GetCarritoAsync(userId);
            if (carritoBd != null && carritoBd.Items != null)
            {
                var detalle = carritoBd.Items.FirstOrDefault(d => d.ProductoId == productoId);
                if (detalle != null)
                {
                    await _carritoConsumer.QuitarProductoAsync(userId, detalle.Id);
                }
            }
            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────────────────
        // VACIAR CARRITO
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Vaciar()
        {
            var userId = GetMyUsuarioId();
            await _carritoConsumer.VaciarCarritoAsync(userId);
            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────────────────
        // CHECKOUT - VISTA DE PAGO
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Checkout()
        {
            var userId = GetMyUsuarioId();
            var carrito = await ObtenerCarritoDtoAsync(userId);
            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            var todasDirecciones = await _direccionConsumer.GetAllAsync();
            var misDirecciones = todasDirecciones.Where(d => d.UsuarioId == userId).ToList();

            if (!misDirecciones.Any())
            {
                TempData["Error"] = "Necesitas registrar una dirección antes de confirmar tu pedido.";
                return RedirectToAction("Index", "ClienteDirecciones");
            }

            ViewBag.Direcciones = new SelectList(misDirecciones, "Id", "Calle");

            // Cargar cupones disponibles para el usuario
            var todosCuponesUsuarios = await _cuponUsuarioConsumer.GetAllAsync();
            var misCuponesUsuarios = todosCuponesUsuarios.Where(cu => cu.UsuarioId == userId && cu.PedidoId == null).ToList();
            
            var cuponesDisponibles = new List<object>();
            foreach (var cu in misCuponesUsuarios)
            {
                if (cu.Cupon != null && cu.Cupon.Activo && System.DateTime.UtcNow <= cu.Cupon.FechaFin)
                {
                    cuponesDisponibles.Add(new {
                        Codigo = cu.Cupon.Codigo,
                        Texto = $"{cu.Cupon.Codigo} - {cu.Cupon.Descripcion} (Vence: {cu.Cupon.FechaFin.ToLocalTime().ToString("dd/MM/yyyy")})"
                    });
                }
            }
            
            ViewBag.Cupones = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(cuponesDisponibles, "Codigo", "Texto");

            return View(carrito);
        }

        [HttpGet]
        public async Task<IActionResult> CalcularCostoEnvio(long direccionId, double? lat, double? lon)
        {
            var userId = GetMyUsuarioId();
            var carrito = await ObtenerCarritoDtoAsync(userId);
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
                return Json(new { exito = true, costo = 1.50m, distancia = 0, tiempo = 0 });
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
        // APLICAR CUPÓN (AJAX)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> AplicarCupon(string codigo)
        {
            var userId = GetMyUsuarioId();
            var carrito = await ObtenerCarritoDtoAsync(userId);
            if (carrito == null || string.IsNullOrWhiteSpace(codigo))
                return Json(new { exito = false, mensaje = "No se proporcionó cupón o carrito vacío." });
            
            var todosCupones = await _cuponConsumer.GetAllAsync();
            var cupon = todosCupones.FirstOrDefault(c => c.Codigo.Equals(codigo.Trim(), System.StringComparison.OrdinalIgnoreCase));

            if (cupon == null) return Json(new { exito = false, mensaje = "El cupón no existe." });
            if (!cupon.Activo) return Json(new { exito = false, mensaje = "El cupón todavía no está activo." });
            if (System.DateTime.UtcNow > cupon.FechaFin) return Json(new { exito = false, mensaje = "El cupón expiró." });
            if (!cupon.EsPublico && cupon.UsuarioExclusivoId != userId) return Json(new { exito = false, mensaje = "Este cupón no pertenece al usuario." });
            if (cupon.PedidoMinimo.HasValue && carrito.Subtotal < cupon.PedidoMinimo.Value)
                return Json(new { exito = false, mensaje = $"No cumple el monto mínimo requerido de ${cupon.PedidoMinimo.Value.ToString("0.00")}." });

            var cuponesUsuarios = await _cuponUsuarioConsumer.GetAllAsync();
            var yaUtilizado = cuponesUsuarios.Any(cu => cu.UsuarioId == userId && cu.CuponId == cupon.Id && cu.PedidoId != null);
            if (yaUtilizado) return Json(new { exito = false, mensaje = "El cupón ya fue utilizado." });
            if (cupon.LimiteUsos.HasValue && cupon.UsosActuales >= cupon.LimiteUsos.Value) return Json(new { exito = false, mensaje = "El cupón alcanzó su límite de usos." });

            decimal montoDescuento = 0;
            if (cupon.TipoDescuento == Delivery.Modelos.Enums.TipoDescuentoEnum.Porcentaje)
            {
                montoDescuento = carrito.Subtotal * (cupon.ValorDescuento / 100m);
                if (cupon.DescuentoMaximo.HasValue && montoDescuento > cupon.DescuentoMaximo.Value)
                {
                    montoDescuento = cupon.DescuentoMaximo.Value;
                }
            }
            else if (cupon.TipoDescuento == Delivery.Modelos.Enums.TipoDescuentoEnum.MontoFijo)
            {
                montoDescuento = cupon.ValorDescuento;
            }
            else 
            {
                montoDescuento = cupon.ValorDescuento; // Fallback
            }

            if (montoDescuento > carrito.Subtotal) montoDescuento = carrito.Subtotal;

            return Json(new { exito = true, mensaje = "✓ Cupón aplicado correctamente.", descuento = montoDescuento });
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
            string? nuevaLatitudStr,
            string? nuevaLongitudStr,
            string? nuevoAlias,
            string? nuevaReferencia,
            bool guardarDireccion,
            string? cuponCodigo)
        {
            var userId = GetMyUsuarioId();
            var carrito = await ObtenerCarritoDtoAsync(userId);
            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            long finalDireccionId = direccionId;
            double? nuevaLatitud = null;
            double? nuevaLongitud = null;

            if (!string.IsNullOrEmpty(nuevaLatitudStr))
            {
                if (double.TryParse(nuevaLatitudStr.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedLat))
                    nuevaLatitud = parsedLat;
            }
            if (!string.IsNullOrEmpty(nuevaLongitudStr))
            {
                if (double.TryParse(nuevaLongitudStr.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedLng))
                    nuevaLongitud = parsedLng;
            }

            // ... Lógica de nueva dirección ...
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

            // ... Lógica Cupones ...
            Delivery.Modelos.Entidades.Cupon? cuponAplicado = null;
            Delivery.Modelos.Entidades.CuponUsuario? registroCuponUsuario = null;

            if (!string.IsNullOrWhiteSpace(cuponCodigo))
            {
                var todosCupones = await _cuponConsumer.GetAllAsync();
                cuponAplicado = todosCupones.FirstOrDefault(c => c.Codigo.Equals(cuponCodigo.Trim(), System.StringComparison.OrdinalIgnoreCase));

                if (cuponAplicado != null && cuponAplicado.Activo && System.DateTime.UtcNow <= cuponAplicado.FechaFin)
                {
                    var cuponesUsuarios = await _cuponUsuarioConsumer.GetAllAsync();
                    registroCuponUsuario = cuponesUsuarios.FirstOrDefault(cu => cu.UsuarioId == userId && cu.CuponId == cuponAplicado.Id);
                    
                    bool esValidoParaUso = true;
                    if (!cuponAplicado.EsPublico && cuponAplicado.UsuarioExclusivoId != userId) esValidoParaUso = false;
                    if (cuponAplicado.PedidoMinimo.HasValue && carrito.Subtotal < cuponAplicado.PedidoMinimo.Value) esValidoParaUso = false;
                    if (registroCuponUsuario != null && registroCuponUsuario.PedidoId != null) esValidoParaUso = false;
                    if (cuponAplicado.LimiteUsos.HasValue && cuponAplicado.UsosActuales >= cuponAplicado.LimiteUsos.Value) esValidoParaUso = false;

                    if (esValidoParaUso)
                    {
                        decimal montoDescuento = 0;
                        if (cuponAplicado.TipoDescuento == Delivery.Modelos.Enums.TipoDescuentoEnum.Porcentaje)
                        {
                            montoDescuento = carrito.Subtotal * (cuponAplicado.ValorDescuento / 100m);
                            if (cuponAplicado.DescuentoMaximo.HasValue && montoDescuento > cuponAplicado.DescuentoMaximo.Value)
                            {
                                montoDescuento = cuponAplicado.DescuentoMaximo.Value;
                            }
                        }
                        else if (cuponAplicado.TipoDescuento == Delivery.Modelos.Enums.TipoDescuentoEnum.MontoFijo)
                        {
                            montoDescuento = cuponAplicado.ValorDescuento;
                        }
                        else 
                        {
                            montoDescuento = cuponAplicado.ValorDescuento; // Fallback
                        }

                        if (montoDescuento > carrito.Subtotal) montoDescuento = carrito.Subtotal;

                        carrito.Descuento = montoDescuento;
                        carrito.CuponId = cuponAplicado.Id;
                    }
                }
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
                // MARCAR CUPÓN COMO USADO
                if (carrito.CuponId.HasValue && cuponAplicado != null)
                {
                    // Incrementar usos globales
                    cuponAplicado.UsosActuales++;
                    await _cuponConsumer.UpdateAsync(cuponAplicado.Id, cuponAplicado);

                    // Registrar que ESTE usuario lo usó
                    if (registroCuponUsuario != null)
                    {
                        registroCuponUsuario.PedidoId = pedidoCreado.Id;
                        registroCuponUsuario.FechaUso = System.DateTime.UtcNow;
                        registroCuponUsuario.Usado = true;
                        await _cuponUsuarioConsumer.UpdateAsync(registroCuponUsuario.Id, registroCuponUsuario);
                    }
                    else
                    {
                        // Si era público pero el usuario no lo "guardó" antes en Mis Cupones
                        await _cuponUsuarioConsumer.CreateAsync(new Delivery.Modelos.Entidades.CuponUsuario
                        {
                            CuponId = cuponAplicado.Id,
                            UsuarioId = userId,
                            PedidoId = pedidoCreado.Id,
                            FechaAsignacion = System.DateTime.UtcNow,
                            FechaUso = System.DateTime.UtcNow,
                            Usado = true,
                            Activo = false // Porque ya se usó, no lo necesitamos activo
                        });
                    }
                }

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
