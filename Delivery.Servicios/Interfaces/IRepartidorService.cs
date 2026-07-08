using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IRepartidorService
    {
        Task<IEnumerable<Repartidor>> GetAllAsync();
        Task<Repartidor?> GetByIdAsync(long id);
        Task<Repartidor> CreateAsync(Repartidor repartidor);
        Task<Repartidor> UpdateAsync(Repartidor repartidor);
        Task<bool> DeleteAsync(long id);
    }
}
