using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IPagoConsumer
    {
        Task<IEnumerable<Pago>> GetAllAsync();
        Task<Pago?> GetByIdAsync(long id);
        Task<Pago> CreateAsync(Pago entity);
        Task<bool> UpdateAsync(long id, Pago entity);
        Task<bool> DeleteAsync(long id);
    }
}
