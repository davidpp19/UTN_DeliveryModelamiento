using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Consumer.Interfaces
{
    public interface INotificacionConsumer
    {
        Task<IEnumerable<NotificacionDto>?> GetMisNotificacionesAsync();
        Task<bool> MarcarComoLeidaAsync(long notificacionId);
    }
}
