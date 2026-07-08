using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IPagoService
    {
        Task<IEnumerable<Pago>> GetAllAsync();
        Task<Pago?> GetByIdAsync(long id);
        Task<Pago> CreateAsync(Pago pago);
        Task<Pago> UpdateAsync(Pago pago);
        Task<bool> DeleteAsync(long id);
    }
}
