using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class PedidoService : IPedidoService
    {
        private readonly DeliveryDbContext _context;

        public PedidoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Pagos)
                .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(long id)
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Pagos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pedido> CreateAsync(Pedido pedido)
        {
            pedido.FechaPedido = System.DateTime.UtcNow;
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<Pedido> UpdateAsync(Pedido pedido)
        {
            var existing = await _context.Pedidos.FindAsync(pedido.Id);
            if (existing == null) return null!;

            existing.UsuarioId = pedido.UsuarioId;
            existing.RestauranteId = pedido.RestauranteId;
            existing.RepartidorId = pedido.RepartidorId;
            existing.DireccionEntregaId = pedido.DireccionEntregaId;
            existing.EstadoPedido = pedido.EstadoPedido;
            existing.Subtotal = pedido.Subtotal;
            existing.CostoEnvio = pedido.CostoEnvio;
            existing.Total = pedido.Total;
            existing.TipoMetodoPago = pedido.TipoMetodoPago;
            existing.MetodoPagoId = pedido.MetodoPagoId;
            existing.CuponId = pedido.CuponId;
            existing.MontoDescuento = pedido.MontoDescuento;
            existing.Notas = pedido.Notas;
            existing.FechaEntregaEstimada = pedido.FechaEntregaEstimada;
            existing.FechaEntregaReal = pedido.FechaEntregaReal;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
