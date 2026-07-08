using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICategoriaProductoConsumer
    {
        Task<IEnumerable<CategoriaProducto>> GetAllAsync();
        Task<CategoriaProducto?> GetByIdAsync(long id);
        Task<CategoriaProducto> CreateAsync(CategoriaProducto entity);
        Task<bool> UpdateAsync(long id, CategoriaProducto entity);
        Task<bool> DeleteAsync(long id);
    }
}
