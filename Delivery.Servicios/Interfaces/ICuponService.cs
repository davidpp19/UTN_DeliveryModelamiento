using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICuponService
    {
        Task<IEnumerable<Cupon>> GetAllAsync();
        Task<Cupon?> GetByIdAsync(long id);
        Task<Cupon> CreateAsync(Cupon cupon);
        Task<Cupon> UpdateAsync(Cupon cupon);
        Task<bool> DeleteAsync(long id);
    }
}
