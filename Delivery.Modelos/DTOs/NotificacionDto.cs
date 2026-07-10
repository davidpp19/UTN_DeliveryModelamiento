using System;

namespace Delivery.Modelos.DTOs
{
    public class NotificacionDto
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public DateTime CreadaEn { get; set; }
    }
}
