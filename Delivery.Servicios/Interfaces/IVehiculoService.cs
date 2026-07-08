using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IVehiculoService
    {
        Task<IEnumerable<Vehiculo>> GetAllAsync();
        Task<Vehiculo?> GetByIdAsync(long id);
        Task<Vehiculo> CreateAsync(Vehiculo vehiculo);
        Task<Vehiculo> UpdateAsync(Vehiculo vehiculo);
        Task<bool> DeleteAsync(long id);
    }
}
