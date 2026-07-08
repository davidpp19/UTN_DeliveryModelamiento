using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriasController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroAuditoria>>> GetAuditorias()
        {
            var registros = await _auditoriaService.GetAllAsync();
            return Ok(registros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroAuditoria>> GetAuditoria(long id)
        {
            var registro = await _auditoriaService.GetByIdAsync(id);
            if (registro == null) return NotFound();
            return Ok(registro);
        }

        [HttpPost]
        public async Task<ActionResult<RegistroAuditoria>> PostAuditoria(RegistroAuditoria registro)
        {
            var created = await _auditoriaService.CreateAsync(registro);
            return CreatedAtAction(nameof(GetAuditoria), new { id = created.Id }, created);
        }
    }
}
