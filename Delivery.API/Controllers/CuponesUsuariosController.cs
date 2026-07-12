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

        [HttpGet("{id}")]
        public async Task<ActionResult<CuponUsuario>> GetCuponUsuario(long id)
        {
            var cuponUsuario = await _cuponUsuarioService.GetByIdAsync(id);
            if (cuponUsuario == null) return NotFound();
            return Ok(cuponUsuario);
        }

        [HttpPost]
        public async Task<ActionResult<CuponUsuario>> PostCuponUsuario(CuponUsuario cuponUsuario)
        {
            var created = await _cuponUsuarioService.CreateAsync(cuponUsuario);
            return CreatedAtAction(nameof(GetCuponUsuario), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuponUsuario(long id, CuponUsuario cuponUsuario)
        {
            if (id != cuponUsuario.Id) return BadRequest();
            var updated = await _cuponUsuarioService.UpdateAsync(cuponUsuario);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuponUsuario(long id)
        {
            var deleted = await _cuponUsuarioService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
