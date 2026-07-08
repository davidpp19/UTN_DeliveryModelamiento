using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IRestauranteConsumer
    {
        Task<IEnumerable<Restaurante>> GetAllAsync();
        Task<Restaurante?> GetByIdAsync(long id);
        Task<Restaurante> CreateAsync(Restaurante entity);
        Task<bool> UpdateAsync(long id, Restaurante entity);
        Task<bool> DeleteAsync(long id);
    }
}
