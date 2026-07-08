using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IVehiculoConsumer
    {
        Task<IEnumerable<Vehiculo>> GetAllAsync();
        Task<Vehiculo?> GetByIdAsync(long id);
        Task<Vehiculo> CreateAsync(Vehiculo entity);
        Task<bool> UpdateAsync(long id, Vehiculo entity);
        Task<bool> DeleteAsync(long id);
    }
}
