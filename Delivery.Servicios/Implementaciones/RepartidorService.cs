using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class RepartidorService : IRepartidorService
    {
        private readonly DeliveryDbContext _context;

        public RepartidorService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Repartidor>> GetAllAsync()
        {
            return await _context.Repartidores
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculos)
                .ToListAsync();
        }

        public async Task<Repartidor?> GetByIdAsync(long id)
        {
            return await _context.Repartidores.FindAsync(id);
        }

        public async Task<Repartidor> CreateAsync(Repartidor repartidor)
        {
            repartidor.CreadoEn = System.DateTime.UtcNow;
            _context.Repartidores.Add(repartidor);
            await _context.SaveChangesAsync();
            return repartidor;
        }

        public async Task<Repartidor> UpdateAsync(Repartidor repartidor)
        {
            var existing = await _context.Repartidores.FindAsync(repartidor.UsuarioId);
            if (existing == null) return null!;

            existing.LicenciaConducir = repartidor.LicenciaConducir;
            existing.FotoLicenciaUrl = repartidor.FotoLicenciaUrl;
            existing.EstadoAprobacion = repartidor.EstadoAprobacion;
            existing.AprobadoPor = repartidor.AprobadoPor;
            existing.FechaAprobacion = repartidor.FechaAprobacion;
            existing.Disponible = repartidor.Disponible;
            existing.CalificacionPromedio = repartidor.CalificacionPromedio;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var repartidor = await _context.Repartidores.FindAsync(id);
            if (repartidor == null) return false;

            _context.Repartidores.Remove(repartidor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
