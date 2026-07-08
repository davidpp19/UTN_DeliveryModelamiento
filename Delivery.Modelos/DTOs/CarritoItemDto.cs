namespace Delivery.Modelos.DTOs
{
    public class CarritoItemDto
    {
        public long ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public string? Notas { get; set; }
    }
}
