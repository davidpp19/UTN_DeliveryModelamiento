using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IFavoritoService
    {
        Task<IEnumerable<Favorito>> GetAllAsync();
        Task<Favorito?> GetByIdsAsync(long usuarioId, long restauranteId);
        Task<Favorito> CreateAsync(Favorito favorito);
        Task<bool> DeleteAsync(long usuarioId, long restauranteId);
    }
}
