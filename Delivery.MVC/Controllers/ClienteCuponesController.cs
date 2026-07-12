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

            // Temp logs
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Intentando registrar código: '{codigo}'");

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(userIdString, out long userId);

            // 1. Validar que el cupón exista
            var todosCupones = await _cuponConsumer.GetAllAsync();
            var cuponesList = todosCupones.ToList();
            
            // DIAGNÓSTICO: Buscar coincidencias de forma manual
            var codigosExistentes = string.Join(", ", cuponesList.Select(c => $"'{c.Codigo}'"));
            var codigoRecibido = codigo?.Trim();
            
            var cupon = cuponesList.FirstOrDefault(c => 
                c.Codigo != null && 
                c.Codigo.Trim().Equals(codigoRecibido, System.StringComparison.OrdinalIgnoreCase));

            if (cupon == null)
            {
                TempData["Error"] = $"DIAGNÓSTICO -> Recibido: '{codigoRecibido}'. Total en BD: {cuponesList.Count}. Códigos BD: [{codigosExistentes}]. El cupón NO fue encontrado por Equals.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Validar que no haya expirado
            if (!cupon.Activo)
            {
                TempData["Error"] = "El cupón se encuentra desactivado.";
                return RedirectToAction(nameof(Index));
            }

            if (System.DateTime.UtcNow > cupon.FechaFin)
            {
                TempData["Error"] = "El cupón ha expirado.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Validar exclusividad
            if (!cupon.EsPublico && cupon.UsuarioExclusivoId != userId)
            {
                TempData["Error"] = $"DIAGNÓSTICO -> Cupón encontrado pero falló exclusividad. EsPublico: {cupon.EsPublico}, UsuarioExclusivoId: {cupon.UsuarioExclusivoId}, TuId: {userId}. Se iba a devolver 'El cupón no existe.'"; 
                return RedirectToAction(nameof(Index));
            }

            // 4. Validar si ya lo registró o lo usó
            var misCupones = await _cuponUsuarioConsumer.GetAllAsync();
            var registroPrevio = misCupones.FirstOrDefault(cu => cu.UsuarioId == userId && cu.CuponId == cupon.Id);
            
            if (registroPrevio != null)
            {
                if (registroPrevio.Usado)
                {
                    TempData["Error"] = "Este cupón ya fue utilizado.";
                }
                else
                {
                    TempData["Error"] = "Este cupón ya fue agregado.";
                }
                return RedirectToAction(nameof(Index));
            }

            // Registrar cupón
            var nuevoRegistro = new Delivery.Modelos.Entidades.CuponUsuario
            {
                CuponId = cupon.Id,
                UsuarioId = userId,
                FechaAsignacion = System.DateTime.UtcNow,
                FechaExpiracion = cupon.FechaFin,
                Usado = false,
                Activo = true
            };

            try
            {
                var registroCreado = await _cuponUsuarioConsumer.CreateAsync(nuevoRegistro);
                TempData["Success"] = $"Cupón agregado correctamente. (Diag: Id generado = {registroCreado?.Id})";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"DIAGNÓSTICO AL GUARDAR -> Error: {ex.Message}. Inner: {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
