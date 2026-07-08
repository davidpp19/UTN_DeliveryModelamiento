using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IHistorialAsignacionesRepartidorConsumer
    {
        Task<IEnumerable<HistorialAsignacionesRepartidor>> GetAllAsync();
        Task<HistorialAsignacionesRepartidor?> GetByIdAsync(long id);
        Task<HistorialAsignacionesRepartidor> CreateAsync(HistorialAsignacionesRepartidor entity);
        Task<bool> UpdateAsync(long id, HistorialAsignacionesRepartidor entity);
        Task<bool> DeleteAsync(long id);
    }
}
