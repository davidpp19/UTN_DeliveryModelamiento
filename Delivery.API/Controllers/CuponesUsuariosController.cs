using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponesUsuariosController : ControllerBase
    {
        private readonly ICuponUsuarioService _cuponUsuarioService;

        public CuponesUsuariosController(ICuponUsuarioService cuponUsuarioService)
        {
            _cuponUsuarioService = cuponUsuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuponUsuario>>> GetCuponesUsuarios()
        {
            var cuponesUsuarios = await _cuponUsuarioService.GetAllAsync();
            return Ok(cuponesUsuarios);
        }

        [HttpGet("{cuponId}/{usuarioId}/{pedidoId?}")]
        public async Task<ActionResult<CuponUsuario>> GetCuponUsuario(long cuponId, long usuarioId, long? pedidoId)
        {
            var cuponUsuario = await _cuponUsuarioService.GetByIdsAsync(cuponId, usuarioId, pedidoId);
            if (cuponUsuario == null) return NotFound();
            return Ok(cuponUsuario);
        }

        [HttpPost]
        public async Task<ActionResult<CuponUsuario>> PostCuponUsuario(CuponUsuario cuponUsuario)
        {
            var created = await _cuponUsuarioService.CreateAsync(cuponUsuario);
            return CreatedAtAction(nameof(GetCuponUsuario), new { cuponId = created.CuponId, usuarioId = created.UsuarioId, pedidoId = created.PedidoId }, created);
        }

        [HttpDelete("{cuponId}/{usuarioId}/{pedidoId?}")]
        public async Task<IActionResult> DeleteCuponUsuario(long cuponId, long usuarioId, long? pedidoId)
        {
            var deleted = await _cuponUsuarioService.DeleteAsync(cuponId, usuarioId, pedidoId);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
