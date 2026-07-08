using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICuponUsuarioService
    {
        Task<IEnumerable<CuponUsuario>> GetAllAsync();
        Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId);
        Task<CuponUsuario> CreateAsync(CuponUsuario cuponUsuario);
        Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId);
    }
}
