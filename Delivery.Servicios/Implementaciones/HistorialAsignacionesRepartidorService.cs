using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class HistorialAsignacionesRepartidorService : IHistorialAsignacionesRepartidorService
    {
        private readonly DeliveryDbContext _context;

        public HistorialAsignacionesRepartidorService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistorialAsignacionesRepartidor>> GetAllAsync()
        {
            return await _context.HistorialAsignaciones.ToListAsync();
        }

        public async Task<HistorialAsignacionesRepartidor?> GetByIdAsync(long id)
        {
            return await _context.HistorialAsignaciones.FindAsync(id);
        }

        public async Task<HistorialAsignacionesRepartidor> CreateAsync(HistorialAsignacionesRepartidor historial)
        {
            historial.CreadoEn = System.DateTime.UtcNow;
            _context.HistorialAsignaciones.Add(historial);
            await _context.SaveChangesAsync();
            return historial;
        }

        public async Task<HistorialAsignacionesRepartidor> UpdateAsync(HistorialAsignacionesRepartidor historial)
        {
            var existing = await _context.HistorialAsignaciones.FindAsync(historial.Id);
            if (existing == null) return null!;

            existing.RepartidorId = historial.RepartidorId;
            existing.PedidoId = historial.PedidoId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var historial = await _context.HistorialAsignaciones.FindAsync(id);
            if (historial == null) return false;

            _context.HistorialAsignaciones.Remove(historial);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
