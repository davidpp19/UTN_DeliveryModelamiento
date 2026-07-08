using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IResenaConsumer
    {
        Task<IEnumerable<Resena>> GetAllAsync();
        Task<Resena?> GetByIdAsync(long id);
        Task<Resena> CreateAsync(Resena entity);
        Task<bool> UpdateAsync(long id, Resena entity);
        Task<bool> DeleteAsync(long id);
    }
}
