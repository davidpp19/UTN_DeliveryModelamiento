using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IRestauranteService
    {
        Task<IEnumerable<Restaurante>> GetAllAsync();
        Task<Restaurante?> GetByIdAsync(long id);
        Task<Restaurante> CreateAsync(Restaurante restaurante);
        Task<Restaurante> UpdateAsync(Restaurante restaurante);
        Task<bool> DeleteAsync(long id);
    }
}
