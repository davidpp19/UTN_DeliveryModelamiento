using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IUbicacionActualRepartidorConsumer
    {
        Task<IEnumerable<UbicacionActualRepartidor>> GetAllAsync();
        Task<UbicacionActualRepartidor?> GetByIdAsync(long id);
        Task<UbicacionActualRepartidor> CreateAsync(UbicacionActualRepartidor entity);
        Task<bool> UpdateAsync(long id, UbicacionActualRepartidor entity);
        Task<bool> DeleteAsync(long id);
    }
}
