using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IUsuarioConsumer
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(long id);
        Task<Usuario?> CreateAsync(Usuario entity);
        Task<bool> UpdateAsync(long id, Usuario entity);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<Delivery.Modelos.Entidades.Notificacion>> GetNotificacionesAsync(long id);
    }
}
