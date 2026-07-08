using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class ProductoService : IProductoService
    {
        private readonly DeliveryDbContext _context;

        public ProductoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos.Include(p => p.Restaurante).Include(p => p.Categoria).ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(long id)
        {
            return await _context.Productos.Include(p => p.Restaurante).Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            producto.CreadoEn = System.DateTime.UtcNow;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> UpdateAsync(Producto producto)
        {
            var existing = await _context.Productos.FindAsync(producto.Id);
            if (existing == null) return null!;

            existing.RestauranteId = producto.RestauranteId;
            existing.CategoriaId = producto.CategoriaId;
            existing.Nombre = producto.Nombre;
            existing.Descripcion = producto.Descripcion;
            existing.Precio = producto.Precio;
            existing.ImagenUrl = producto.ImagenUrl;
            existing.Disponible = producto.Disponible;
            existing.TiempoPreparacion = producto.TiempoPreparacion;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
