using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IFavoritoConsumer
    {
        Task<IEnumerable<Favorito>> GetAllAsync();
        Task<Favorito?> GetByIdsAsync(long usuarioId, long restauranteId);
        Task<Favorito> CreateAsync(Favorito entity);
        Task<bool> DeleteAsync(long usuarioId, long restauranteId);
    }
}
