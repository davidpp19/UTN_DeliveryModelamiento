using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class DireccionService : IDireccionService
    {
        private readonly DeliveryDbContext _context;

        public DireccionService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Direccion>> GetAllAsync()
        {
            return await _context.Direcciones.Include(d => d.Usuario).ToListAsync();
        }

        public async Task<Direccion?> GetByIdAsync(long id)
        {
            return await _context.Direcciones.Include(d => d.Usuario).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Direccion> CreateAsync(Direccion direccion)
        {
            direccion.CreadoEn = System.DateTime.UtcNow;
            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();
            return direccion;
        }

        public async Task<Direccion> UpdateAsync(Direccion direccion)
        {
            var existingDireccion = await _context.Direcciones.FindAsync(direccion.Id);
            if (existingDireccion == null) return null!;

            existingDireccion.UsuarioId = direccion.UsuarioId;
            existingDireccion.Alias = direccion.Alias;
            existingDireccion.Calle = direccion.Calle;
            existingDireccion.Numero = direccion.Numero;
            existingDireccion.Ciudad = direccion.Ciudad;
            existingDireccion.Referencia = direccion.Referencia;
            existingDireccion.Latitud = direccion.Latitud;
            existingDireccion.Longitud = direccion.Longitud;
            existingDireccion.EsPrincipal = direccion.EsPrincipal;
            existingDireccion.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingDireccion;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var direccion = await _context.Direcciones.FindAsync(id);
            if (direccion == null) return false;

            _context.Direcciones.Remove(direccion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
