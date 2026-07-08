using System.Collections.Generic;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class MensajeNotificacionDto
    {
        public long UsuarioDestinoId { get; set; }
        public string Destinatario { get; set; } = string.Empty; // Email, Phone, or DeviceToken
        public string Asunto { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public CanalNotificacionEnum Canal { get; set; }
        public Dictionary<string, string> DatosAdicionales { get; set; } = new Dictionary<string, string>();
    }
}
