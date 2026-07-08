using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class PagoService : IPagoService
    {
        private readonly DeliveryDbContext _context;

        public PagoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pago>> GetAllAsync()
        {
            return await _context.Pagos.ToListAsync();
        }

        public async Task<Pago?> GetByIdAsync(long id)
        {
            return await _context.Pagos.FindAsync(id);
        }

        public async Task<Pago> CreateAsync(Pago pago)
        {
            pago.CreadoEn = System.DateTime.UtcNow;
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<Pago> UpdateAsync(Pago pago)
        {
            var existing = await _context.Pagos.FindAsync(pago.Id);
            if (existing == null) return null!;

            existing.PedidoId = pago.PedidoId;
            existing.TipoMetodoPago = pago.TipoMetodoPago;
            existing.Monto = pago.Monto;
            existing.EstadoPago = pago.EstadoPago;
            existing.ReferenciaTransaccion = pago.ReferenciaTransaccion;
            existing.FechaPago = pago.FechaPago;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return false;

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
