using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICuponUsuarioService
    {
        Task<IEnumerable<CuponUsuario>> GetAllAsync();
        Task<CuponUsuario?> GetByIdAsync(long id);
        Task<CuponUsuario> CreateAsync(CuponUsuario cuponUsuario);
        Task<CuponUsuario?> UpdateAsync(CuponUsuario cuponUsuario);
        Task<bool> DeleteAsync(long id);
    }
}
