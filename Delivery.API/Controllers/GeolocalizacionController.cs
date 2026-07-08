using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocalizacionController : ControllerBase
    {
        private readonly IGeolocalizacionService _geolocalizacionService;

        public GeolocalizacionController(IGeolocalizacionService geolocalizacionService)
        {
            _geolocalizacionService = geolocalizacionService;
        }

        [HttpGet("repartidor/{repartidorId}")]
        public async Task<ActionResult<CoordenadasDto>> ObtenerUbicacion(long repartidorId)
        {
            var ubicacion = await _geolocalizacionService.ObtenerUbicacionRepartidorAsync(repartidorId);
            if (ubicacion == null) return NotFound();
            return Ok(ubicacion);
        }

        [HttpPost("repartidor")]
        public async Task<IActionResult> ActualizarUbicacion(ActualizacionUbicacionDto actualizacion)
        {
            await _geolocalizacionService.ActualizarUbicacionRepartidorAsync(actualizacion);
            return Ok();
        }
    }
}
