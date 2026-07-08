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

        public async Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            return await _context.CuponesUsuarios
                .FirstOrDefaultAsync(cu => cu.CuponId == cuponId && cu.UsuarioId == usuarioId && cu.PedidoId == pedidoId);
        }

        public async Task<CuponUsuario> CreateAsync(CuponUsuario cuponUsuario)
        {
            cuponUsuario.FechaUso = System.DateTime.UtcNow;
            _context.CuponesUsuarios.Add(cuponUsuario);
            await _context.SaveChangesAsync();
            return cuponUsuario;
        }

        public async Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            var cuponUsuario = await GetByIdsAsync(cuponId, usuarioId, pedidoId);
            if (cuponUsuario == null) return false;

            _context.CuponesUsuarios.Remove(cuponUsuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
