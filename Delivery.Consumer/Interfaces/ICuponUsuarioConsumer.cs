using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICuponUsuarioConsumer
    {
        Task<IEnumerable<CuponUsuario>> GetAllAsync();
        Task<CuponUsuario?> GetByIdAsync(long id);
        Task<CuponUsuario> CreateAsync(CuponUsuario entity);
        Task<bool> UpdateAsync(long id, CuponUsuario entity);
        Task<bool> DeleteAsync(long id);
    }
}
