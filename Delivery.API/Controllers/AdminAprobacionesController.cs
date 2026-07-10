using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAprobacionesController : ControllerBase
    {
        private readonly IRepartidorService _repartidorService;
        private readonly IRestauranteService _restauranteService;
        private readonly INotificacionService _notificacionService;
        private readonly IUsuarioService _usuarioService;

        public AdminAprobacionesController(
            IRepartidorService repartidorService,
            IRestauranteService restauranteService,
            INotificacionService notificacionService,
            IUsuarioService usuarioService)
        {
            _repartidorService = repartidorService;
            _restauranteService = restauranteService;
            _notificacionService = notificacionService;
            _usuarioService = usuarioService;
        }

        private long GetAdminId()
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long id))
            {
                return id;
            }
            return 1; // Default fallback
        }

        [HttpGet("repartidores/pendientes")]
        public async Task<IActionResult> GetRepartidoresPendientes()
        {
            var todos = await _repartidorService.GetAllAsync();
            var pendientes = todos
                .Where(r => r.EstadoAprobacion == EstadoAprobacionEnum.Pendiente)
                .Select(r => new RepartidorPendienteDto
                {
                    Id = r.UsuarioId,
                    UsuarioId = r.UsuarioId,
                    Nombres = r.Usuario?.Nombre ?? "",
                    Apellidos = r.Usuario?.Apellidos ?? "",
                    Email = r.Usuario?.Email ?? "",
                    Telefono = r.Usuario?.Telefono ?? "",
                    Cedula = r.Usuario?.Cedula ?? "",
                    LicenciaConducir = r.LicenciaConducir,
                    FotoLicenciaUrl = r.FotoLicenciaUrl,
                    CreadoEn = r.CreadoEn,
                    TipoVehiculo = r.Vehiculos?.FirstOrDefault()?.TipoVehiculo.ToString() ?? "",
                    Placa = r.Vehiculos?.FirstOrDefault()?.Placa ?? "",
                    FotoPerfilUrl = r.Usuario?.FotoPerfilUrl
                })
                .ToList();
            return Ok(pendientes);
        }

        [HttpGet("restaurantes/pendientes")]
        public async Task<IActionResult> GetRestaurantesPendientes()
        {
            var todos = await _restauranteService.GetAllAsync();
            var filtrados = todos.Where(r => r.Estado == EstadoRestauranteEnum.Pendiente).ToList();
            
            var pendientes = new System.Collections.Generic.List<RestaurantePendienteDto>();
            foreach(var r in filtrados)
            {
                var usuario = await _usuarioService.GetByIdAsync(r.CreadoPor ?? 0);
                pendientes.Add(new RestaurantePendienteDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Categoria = r.Categoria ?? "",
                    Ruc = r.Ruc ?? "",
                    Calle = r.Calle ?? "",
                    Ciudad = r.Ciudad ?? "",
                    Telefono = r.Telefono,
                    Email = r.Email,
                    CreadoEn = r.CreadoEn,
                    NombresPropietario = usuario?.Nombre ?? "",
                    ApellidosPropietario = usuario?.Apellidos ?? "",
                    EmailPropietario = usuario?.Email ?? "",
                    LogoUrl = r.LogoUrl,
                    FotoPerfilUrl = usuario?.FotoPerfilUrl
                });
            }
            return Ok(pendientes);
        }

        [HttpPost("repartidores/{id}/aprobar")]
        public async Task<IActionResult> AprobarRepartidor(long id)
        {
            var repartidor = await _repartidorService.GetByIdAsync(id);
            if (repartidor == null) return NotFound();

            repartidor.EstadoAprobacion = EstadoAprobacionEnum.Aprobado;
            repartidor.AprobadoPor = GetAdminId();
            repartidor.FechaAprobacion = DateTime.UtcNow;
            await _repartidorService.UpdateAsync(repartidor);

            await _notificacionService.EnviarNotificacionAsync(new MensajeNotificacionDto
            {
                UsuarioDestinoId = repartidor.UsuarioId,
                Asunto = "¡Cuenta Aprobada!",
                Contenido = "Tu solicitud como repartidor ha sido aprobada. ¡Ya puedes empezar a recibir pedidos!",
                Canal = CanalNotificacionEnum.Push
            });

            return Ok(new { message = "Repartidor aprobado exitosamente." });
        }

        [HttpPost("repartidores/{id}/rechazar")]
        public async Task<IActionResult> RechazarRepartidor(long id, [FromQuery] string? motivo = null)
        {
            var repartidor = await _repartidorService.GetByIdAsync(id);
            if (repartidor == null) return NotFound();

            repartidor.EstadoAprobacion = EstadoAprobacionEnum.Rechazado;
            await _repartidorService.UpdateAsync(repartidor);

            var textoMotivo = !string.IsNullOrEmpty(motivo) ? $" Motivo: {motivo}" : "";

            await _notificacionService.EnviarNotificacionAsync(new MensajeNotificacionDto
            {
                UsuarioDestinoId = repartidor.UsuarioId,
                Asunto = "Solicitud Rechazada",
                Contenido = $"Lamentablemente tu solicitud como repartidor ha sido rechazada.{textoMotivo}",
                Canal = CanalNotificacionEnum.Email
            });

            return Ok(new { message = "Repartidor rechazado." });
        }

        [HttpPost("restaurantes/{id}/aprobar")]
        public async Task<IActionResult> AprobarRestaurante(long id)
        {
            var restaurante = await _restauranteService.GetByIdAsync(id);
            if (restaurante == null) return NotFound();

            restaurante.Estado = EstadoRestauranteEnum.Aprobado;
            restaurante.AprobadoPor = GetAdminId();
            restaurante.FechaAprobacion = DateTime.UtcNow;
            await _restauranteService.UpdateAsync(restaurante);

            if (restaurante.CreadoPor.HasValue)
            {
                await _notificacionService.EnviarNotificacionAsync(new MensajeNotificacionDto
                {
                    UsuarioDestinoId = restaurante.CreadoPor.Value,
                    Asunto = "¡Restaurante Aprobado!",
                    Contenido = "Tu restaurante ha sido aprobado y ya es visible en la plataforma.",
                    Canal = CanalNotificacionEnum.Push
                });
            }

            return Ok(new { message = "Restaurante aprobado exitosamente." });
        }

        [HttpPost("restaurantes/{id}/rechazar")]
        public async Task<IActionResult> RechazarRestaurante(long id, [FromQuery] string? motivo = null)
        {
            var restaurante = await _restauranteService.GetByIdAsync(id);
            if (restaurante == null) return NotFound();

            restaurante.Estado = EstadoRestauranteEnum.Rechazado;
            await _restauranteService.UpdateAsync(restaurante);

            var textoMotivo = !string.IsNullOrEmpty(motivo) ? $" Motivo: {motivo}" : "";

            if (restaurante.CreadoPor.HasValue)
            {
                await _notificacionService.EnviarNotificacionAsync(new MensajeNotificacionDto
                {
                    UsuarioDestinoId = restaurante.CreadoPor.Value,
                    Asunto = "Restaurante Rechazado",
                    Contenido = $"La solicitud de registro de tu restaurante ha sido rechazada.{textoMotivo}",
                    Canal = CanalNotificacionEnum.Email
                });
            }

            return Ok(new { message = "Restaurante rechazado." });
        }
    }
}
