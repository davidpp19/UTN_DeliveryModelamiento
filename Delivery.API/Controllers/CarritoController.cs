using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<Carrito>> GetCarrito(long usuarioId)
        {
            var carrito = await _carritoService.ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null) return NotFound(new { message = "No hay un carrito activo para este usuario." });
            
            return Ok(carrito);
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Carrito>> AgregarProducto([FromBody] AgregarAlCarritoDto dto)
        {
            var carrito = await _carritoService.AgregarProductoAsync(dto);
            return Ok(carrito);
        }

        [HttpDelete("{usuarioId}/quitar/{carritoItemId}")]
        public async Task<IActionResult> QuitarProducto(long usuarioId, long carritoItemId)
        {
            var resultado = await _carritoService.QuitarProductoAsync(usuarioId, carritoItemId);
            if (!resultado) return NotFound();

            return NoContent();
        }

        [HttpDelete("{usuarioId}/vaciar")]
        public async Task<IActionResult> VaciarCarrito(long usuarioId)
        {
            var resultado = await _carritoService.VaciarCarritoAsync(usuarioId);
            if (!resultado) return NotFound();

            return NoContent();
        }
    }
}
