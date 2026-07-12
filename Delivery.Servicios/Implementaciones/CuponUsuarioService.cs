using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class CuponUsuarioService : ICuponUsuarioService
    {
        private readonly DeliveryDbContext _context;

        public CuponUsuarioService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CuponUsuario>> GetAllAsync()
        {
            return await _context.CuponesUsuarios.ToListAsync();
        }

        public async Task<CuponUsuario?> GetByIdAsync(long id)
        {
            return await _context.CuponesUsuarios.FindAsync(id);
        }

        public async Task<CuponUsuario> CreateAsync(CuponUsuario cuponUsuario)
        {
            _context.CuponesUsuarios.Add(cuponUsuario);
            await _context.SaveChangesAsync();
            return cuponUsuario;
        }

        public async Task<CuponUsuario?> UpdateAsync(CuponUsuario cuponUsuario)
        {
            var existing = await _context.CuponesUsuarios.FindAsync(cuponUsuario.Id);
            if (existing == null) return null;

            existing.PedidoId = cuponUsuario.PedidoId;
            existing.Usado = cuponUsuario.Usado;
            existing.Activo = cuponUsuario.Activo;
            existing.FechaUso = cuponUsuario.FechaUso;
            existing.FechaExpiracion = cuponUsuario.FechaExpiracion;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var cuponUsuario = await _context.CuponesUsuarios.FindAsync(id);
            if (cuponUsuario == null) return false;

            _context.CuponesUsuarios.Remove(cuponUsuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
