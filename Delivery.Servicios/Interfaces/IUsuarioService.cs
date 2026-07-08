using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(long id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(long id);
    }
}
