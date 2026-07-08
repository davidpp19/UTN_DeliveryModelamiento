using System;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class NotificacionService : INotificacionService
    {
        // En un futuro, aquí se inyectarán las fábricas o clientes de Stripe, Firebase, Twilio, SendGrid, etc.

        public NotificacionService()
        {
        }

        public async Task EnviarNotificacionAsync(MensajeNotificacionDto mensaje)
        {
            // Simulación arquitectónica (Facade / Adapter preparado)
            // Switch case basado en el mensaje.Canal
            
            /*
            switch (mensaje.Canal)
            {
                case Modelos.Enums.CanalNotificacionEnum.Email:
                    // Await _emailService.SendAsync(...)
                    break;
                case Modelos.Enums.CanalNotificacionEnum.SMS:
                    // Await _smsService.SendAsync(...)
                    break;
                case Modelos.Enums.CanalNotificacionEnum.WhatsApp:
                    // Await _whatsappService.SendAsync(...)
                    break;
                case Modelos.Enums.CanalNotificacionEnum.Push:
                    // Await _firebaseService.SendPushAsync(...)
                    break;
                case Modelos.Enums.CanalNotificacionEnum.SignalR:
                    // Await _signalRHub.Clients.User(userId).SendAsync(...)
                    break;
            }
            */

            await Task.CompletedTask;
        }

        public async Task EnviarNotificacionMultiCanalAsync(MensajeNotificacionDto mensaje, params Delivery.Modelos.Enums.CanalNotificacionEnum[] canales)
        {
            foreach (var canal in canales)
            {
                mensaje.Canal = canal;
                await EnviarNotificacionAsync(mensaje);
            }
        }
    }
}
