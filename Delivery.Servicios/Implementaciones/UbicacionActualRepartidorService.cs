using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class UbicacionActualRepartidorService : IUbicacionActualRepartidorService
    {
        private readonly DeliveryDbContext _context;

        public UbicacionActualRepartidorService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UbicacionActualRepartidor>> GetAllAsync()
        {
            return await _context.UbicacionesActuales.ToListAsync();
        }

        public async Task<UbicacionActualRepartidor?> GetByIdAsync(long id)
        {
            return await _context.UbicacionesActuales.FindAsync(id);
        }

        public async Task<UbicacionActualRepartidor> CreateAsync(UbicacionActualRepartidor ubicacion)
        {
            ubicacion.ActualizadoEn = System.DateTime.UtcNow;
            _context.UbicacionesActuales.Add(ubicacion);
            await _context.SaveChangesAsync();
            return ubicacion;
        }

        public async Task<UbicacionActualRepartidor> UpdateAsync(UbicacionActualRepartidor ubicacion)
        {
            var existing = await _context.UbicacionesActuales.FindAsync(ubicacion.RepartidorId);
            if (existing == null) return null!;

            existing.Latitud = ubicacion.Latitud;
            existing.Longitud = ubicacion.Longitud;
            existing.Rumbo = ubicacion.Rumbo;
            existing.Velocidad = ubicacion.Velocidad;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var ubicacion = await _context.UbicacionesActuales.FindAsync(id);
            if (ubicacion == null) return false;

            _context.UbicacionesActuales.Remove(ubicacion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
