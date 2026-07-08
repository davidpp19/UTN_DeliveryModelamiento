using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class ResenaService : IResenaService
    {
        private readonly DeliveryDbContext _context;

        public ResenaService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resena>> GetAllAsync()
        {
            return await _context.Resenas.ToListAsync();
        }

        public async Task<Resena?> GetByIdAsync(long id)
        {
            return await _context.Resenas.FindAsync(id);
        }

        public async Task<Resena> CreateAsync(Resena resena)
        {
            resena.FechaResena = System.DateTime.UtcNow;
            _context.Resenas.Add(resena);
            await _context.SaveChangesAsync();
            return resena;
        }

        public async Task<Resena> UpdateAsync(Resena resena)
        {
            var existing = await _context.Resenas.FindAsync(resena.Id);
            if (existing == null) return null!;

            existing.PedidoId = resena.PedidoId;
            existing.UsuarioId = resena.UsuarioId;
            existing.RestauranteId = resena.RestauranteId;
            existing.RepartidorId = resena.RepartidorId;
            existing.CalificacionRestaurante = resena.CalificacionRestaurante;
            existing.ComentarioRestaurante = resena.ComentarioRestaurante;
            existing.CalificacionRepartidor = resena.CalificacionRepartidor;
            existing.ComentarioRepartidor = resena.ComentarioRepartidor;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var resena = await _context.Resenas.FindAsync(id);
            if (resena == null) return false;

            _context.Resenas.Remove(resena);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
