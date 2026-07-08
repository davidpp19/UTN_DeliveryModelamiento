using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IDireccionService
    {
        Task<IEnumerable<Direccion>> GetAllAsync();
        Task<Direccion?> GetByIdAsync(long id);
        Task<Direccion> CreateAsync(Direccion direccion);
        Task<Direccion> UpdateAsync(Direccion direccion);
        Task<bool> DeleteAsync(long id);
    }
}
