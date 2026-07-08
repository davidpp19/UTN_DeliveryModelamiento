using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class VehiculoService : IVehiculoService
    {
        private readonly DeliveryDbContext _context;

        public VehiculoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehiculo>> GetAllAsync()
        {
            return await _context.Vehiculos.ToListAsync();
        }

        public async Task<Vehiculo?> GetByIdAsync(long id)
        {
            return await _context.Vehiculos.FindAsync(id);
        }

        public async Task<Vehiculo> CreateAsync(Vehiculo vehiculo)
        {
            vehiculo.CreadoEn = System.DateTime.UtcNow;
            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();
            return vehiculo;
        }

        public async Task<Vehiculo> UpdateAsync(Vehiculo vehiculo)
        {
            var existing = await _context.Vehiculos.FindAsync(vehiculo.Id);
            if (existing == null) return null!;

            existing.RepartidorId = vehiculo.RepartidorId;
            existing.TipoVehiculo = vehiculo.TipoVehiculo;
            existing.Placa = vehiculo.Placa;
            existing.Marca = vehiculo.Marca;
            existing.Modelo = vehiculo.Modelo;
            existing.Anio = vehiculo.Anio;
            existing.Color = vehiculo.Color;
            existing.FotoVehiculoUrl = vehiculo.FotoVehiculoUrl;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return false;

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
