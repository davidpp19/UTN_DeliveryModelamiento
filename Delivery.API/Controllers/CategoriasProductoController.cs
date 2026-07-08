using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasProductoController : ControllerBase
    {
        private readonly ICategoriaProductoService _categoriaProductoService;

        public CategoriasProductoController(ICategoriaProductoService categoriaProductoService)
        {
            _categoriaProductoService = categoriaProductoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaProducto>>> GetCategoriasProducto()
        {
            var categorias = await _categoriaProductoService.GetAllAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaProducto>> GetCategoriaProducto(long id)
        {
            var categoria = await _categoriaProductoService.GetByIdAsync(id);
            if (categoria == null) return NotFound();
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaProducto>> PostCategoriaProducto(CategoriaProducto categoriaProducto)
        {
            var created = await _categoriaProductoService.CreateAsync(categoriaProducto);
            return CreatedAtAction(nameof(GetCategoriaProducto), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaProducto(long id, CategoriaProducto categoriaProducto)
        {
            if (id != categoriaProducto.Id) return BadRequest();
            
            var updated = await _categoriaProductoService.UpdateAsync(categoriaProducto);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaProducto(long id)
        {
            var deleted = await _categoriaProductoService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
