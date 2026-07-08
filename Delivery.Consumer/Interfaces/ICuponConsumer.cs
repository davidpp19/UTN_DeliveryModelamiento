using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICuponConsumer
    {
        Task<IEnumerable<Cupon>> GetAllAsync();
        Task<Cupon?> GetByIdAsync(long id);
        Task<Cupon> CreateAsync(Cupon entity);
        Task<bool> UpdateAsync(long id, Cupon entity);
        Task<bool> DeleteAsync(long id);
    }
}
