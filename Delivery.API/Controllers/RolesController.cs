using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            var roles = await _rolService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(long id)
        {
            var rol = await _rolService.GetByIdAsync(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            var createdRol = await _rolService.CreateAsync(rol);
            return CreatedAtAction(nameof(GetRol), new { id = createdRol.Id }, createdRol);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(long id, Rol rol)
        {
            if (id != rol.Id) return BadRequest();
            
            var updatedRol = await _rolService.UpdateAsync(rol);
            if (updatedRol == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(long id)
        {
            var deleted = await _rolService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
