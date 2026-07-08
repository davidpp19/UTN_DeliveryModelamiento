using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class DetallePedidoService : IDetallePedidoService
    {
        private readonly DeliveryDbContext _context;

        public DetallePedidoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DetallePedido>> GetAllAsync()
        {
            return await _context.DetallesPedido.Include(d => d.Producto).ToListAsync();
        }

        public async Task<DetallePedido?> GetByIdAsync(long id)
        {
            return await _context.DetallesPedido.Include(d => d.Producto).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DetallePedido> CreateAsync(DetallePedido detalle)
        {
            _context.DetallesPedido.Add(detalle);
            await _context.SaveChangesAsync();
            return detalle;
        }

        public async Task<DetallePedido> UpdateAsync(DetallePedido detalle)
        {
            var existing = await _context.DetallesPedido.FindAsync(detalle.Id);
            if (existing == null) return null!;

            existing.PedidoId = detalle.PedidoId;
            existing.ProductoId = detalle.ProductoId;
            existing.Cantidad = detalle.Cantidad;
            existing.PrecioUnitario = detalle.PrecioUnitario;
            existing.Subtotal = detalle.Subtotal;
            existing.NotasEspeciales = detalle.NotasEspeciales;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle == null) return false;

            _context.DetallesPedido.Remove(detalle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
