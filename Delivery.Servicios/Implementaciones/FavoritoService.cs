using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class FavoritoService : IFavoritoService
    {
        private readonly DeliveryDbContext _context;

        public FavoritoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorito>> GetAllAsync()
        {
            return await _context.Favoritos.ToListAsync();
        }

        public async Task<Favorito?> GetByIdsAsync(long usuarioId, long restauranteId)
        {
            return await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.RestauranteId == restauranteId);
        }

        public async Task<Favorito> CreateAsync(Favorito favorito)
        {
            favorito.FechaAgregado = System.DateTime.UtcNow;
            _context.Favoritos.Add(favorito);
            await _context.SaveChangesAsync();
            return favorito;
        }

        public async Task<bool> DeleteAsync(long usuarioId, long restauranteId)
        {
            var favorito = await GetByIdsAsync(usuarioId, restauranteId);
            if (favorito == null) return false;

            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
