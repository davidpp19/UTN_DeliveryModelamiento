using System;

namespace Delivery.Modelos.DTOs
{
    public class ActualizacionUbicacionDto
    {
        public long RepartidorId { get; set; }
        public long? PedidoActualId { get; set; }
        public CoordenadasDto Ubicacion { get; set; } = new CoordenadasDto();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
