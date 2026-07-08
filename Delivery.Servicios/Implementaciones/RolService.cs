using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class RolService : IRolService
    {
        private readonly DeliveryDbContext _context;

        public RolService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(long id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Rol> CreateAsync(Rol rol)
        {
            rol.CreadoEn = System.DateTime.UtcNow;
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<Rol> UpdateAsync(Rol rol)
        {
            var existingRol = await _context.Roles.FindAsync(rol.Id);
            if (existingRol == null) return null!;

            existingRol.Nombre = rol.Nombre;
            existingRol.Descripcion = rol.Descripcion;
            existingRol.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingRol;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
