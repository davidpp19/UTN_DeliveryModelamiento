using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface INotificacionService
    {
        Task EnviarNotificacionAsync(MensajeNotificacionDto mensaje);
        Task EnviarNotificacionMultiCanalAsync(MensajeNotificacionDto mensaje, params Delivery.Modelos.Enums.CanalNotificacionEnum[] canales);
        Task<System.Collections.Generic.IEnumerable<Delivery.Modelos.DTOs.NotificacionDto>> GetNotificacionesByUsuarioAsync(long usuarioId);
        Task MarcarComoLeidaAsync(long notificacionId, long usuarioId);
    }
}

