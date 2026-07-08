using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IRolConsumer
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(long id);
        Task<Rol> CreateAsync(Rol entity);
        Task<bool> UpdateAsync(long id, Rol entity);
        Task<bool> DeleteAsync(long id);
    }
}
