using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IRepartidorConsumer
    {
        Task<IEnumerable<Repartidor>> GetAllAsync();
        Task<Repartidor?> GetByIdAsync(long id);
        Task<Repartidor> CreateAsync(Repartidor entity);
        Task<bool> UpdateAsync(long id, Repartidor entity);
        Task<bool> DeleteAsync(long id);
    }
}
