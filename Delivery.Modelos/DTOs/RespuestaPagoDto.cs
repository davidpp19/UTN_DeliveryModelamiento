using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class RespuestaPagoDto
    {
        public bool Exitoso { get; set; }
        public string TransaccionId { get; set; } = string.Empty;
        public EstadoPagoEnum Estado { get; set; }
        public string MensajeError { get; set; } = string.Empty;
    }
}
