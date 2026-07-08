using System.Collections.Generic;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class SolicitudPagoDto
    {
        public long PedidoId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "USD";
        public TipoMetodoPagoEnum MetodoPago { get; set; }
        
        // Datos adicionales para la pasarela (tokens, correo, etc.)
        public Dictionary<string, string> Parametros { get; set; } = new Dictionary<string, string>();
    }
}
