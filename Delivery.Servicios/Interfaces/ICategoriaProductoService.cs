using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICategoriaProductoService
    {
        Task<IEnumerable<CategoriaProducto>> GetAllAsync();
        Task<CategoriaProducto?> GetByIdAsync(long id);
        Task<CategoriaProducto> CreateAsync(CategoriaProducto categoriaProducto);
        Task<CategoriaProducto> UpdateAsync(CategoriaProducto categoriaProducto);
        Task<bool> DeleteAsync(long id);
    }
}
