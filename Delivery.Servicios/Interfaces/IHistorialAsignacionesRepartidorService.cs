using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IHistorialAsignacionesRepartidorService
    {
        Task<IEnumerable<HistorialAsignacionesRepartidor>> GetAllAsync();
        Task<HistorialAsignacionesRepartidor?> GetByIdAsync(long id);
        Task<HistorialAsignacionesRepartidor> CreateAsync(HistorialAsignacionesRepartidor historial);
        Task<HistorialAsignacionesRepartidor> UpdateAsync(HistorialAsignacionesRepartidor historial);
        Task<bool> DeleteAsync(long id);
    }
}
