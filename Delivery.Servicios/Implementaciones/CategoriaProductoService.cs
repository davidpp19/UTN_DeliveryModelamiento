using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class CategoriaProductoService : ICategoriaProductoService
    {
        private readonly DeliveryDbContext _context;

        public CategoriaProductoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaProducto>> GetAllAsync()
        {
            return await _context.CategoriasProducto.Include(c => c.Restaurante).ToListAsync();
        }

        public async Task<CategoriaProducto?> GetByIdAsync(long id)
        {
            return await _context.CategoriasProducto.Include(c => c.Restaurante).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CategoriaProducto> CreateAsync(CategoriaProducto categoriaProducto)
        {
            categoriaProducto.CreadoEn = System.DateTime.UtcNow;
            _context.CategoriasProducto.Add(categoriaProducto);
            await _context.SaveChangesAsync();
            return categoriaProducto;
        }

        public async Task<CategoriaProducto> UpdateAsync(CategoriaProducto categoriaProducto)
        {
            var existing = await _context.CategoriasProducto.FindAsync(categoriaProducto.Id);
            if (existing == null) return null!;

            existing.RestauranteId = categoriaProducto.RestauranteId;
            existing.Nombre = categoriaProducto.Nombre;
            existing.Descripcion = categoriaProducto.Descripcion;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var categoriaProducto = await _context.CategoriasProducto.FindAsync(id);
            if (categoriaProducto == null) return false;

            _context.CategoriasProducto.Remove(categoriaProducto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
