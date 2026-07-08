using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICuponUsuarioConsumer
    {
        Task<IEnumerable<CuponUsuario>> GetAllAsync();
        Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId);
        Task<CuponUsuario> CreateAsync(CuponUsuario entity);
        Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId);
    }
}
