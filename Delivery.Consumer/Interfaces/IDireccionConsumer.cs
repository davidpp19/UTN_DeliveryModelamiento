using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IDireccionConsumer
    {
        Task<IEnumerable<Direccion>> GetAllAsync();
        Task<Direccion?> GetByIdAsync(long id);
        Task<Direccion> CreateAsync(Direccion entity);
        Task<bool> UpdateAsync(long id, Direccion entity);
        Task<bool> DeleteAsync(long id);
    }
}
