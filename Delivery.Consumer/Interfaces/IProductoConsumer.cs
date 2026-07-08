using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IProductoConsumer
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(long id);
        Task<Producto> CreateAsync(Producto entity);
        Task<bool> UpdateAsync(long id, Producto entity);
        Task<bool> DeleteAsync(long id);
    }
}
