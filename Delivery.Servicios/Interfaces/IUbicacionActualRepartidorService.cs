using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IUbicacionActualRepartidorService
    {
        Task<IEnumerable<UbicacionActualRepartidor>> GetAllAsync();
        Task<UbicacionActualRepartidor?> GetByIdAsync(long id);
        Task<UbicacionActualRepartidor> CreateAsync(UbicacionActualRepartidor ubicacion);
        Task<UbicacionActualRepartidor> UpdateAsync(UbicacionActualRepartidor ubicacion);
        Task<bool> DeleteAsync(long id);
    }
}
