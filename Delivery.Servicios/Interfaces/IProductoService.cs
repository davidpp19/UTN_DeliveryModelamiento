using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(long id);
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(long id);
    }
}
