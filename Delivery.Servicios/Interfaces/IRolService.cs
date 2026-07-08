using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(long id);
        Task<Rol> CreateAsync(Rol rol);
        Task<Rol> UpdateAsync(Rol rol);
        Task<bool> DeleteAsync(long id);
    }
}
