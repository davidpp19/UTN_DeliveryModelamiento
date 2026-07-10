using System.Threading.Tasks;
using System.Collections.Generic;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface INotificacionService
    {
        Task<bool> EnviarNotificacionAsync(MensajeNotificacionDto mensajeDto);
        Task<IEnumerable<Delivery.Modelos.Entidades.Notificacion>> GetByUsuarioIdAsync(long usuarioId);
        Task EnviarNotificacionMultiCanalAsync(MensajeNotificacionDto mensaje, params Delivery.Modelos.Enums.CanalNotificacionEnum[] canales);
        Task<System.Collections.Generic.IEnumerable<Delivery.Modelos.DTOs.NotificacionDto>> GetNotificacionesByUsuarioAsync(long usuarioId);
        Task MarcarComoLeidaAsync(long notificacionId, long usuarioId);
    }
}
