using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IResenaService
    {
        Task<IEnumerable<Resena>> GetAllAsync();
        Task<Resena?> GetByIdAsync(long id);
        Task<Resena> CreateAsync(Resena resena);
        Task<Resena> UpdateAsync(Resena resena);
        Task<bool> DeleteAsync(long id);
    }
}
