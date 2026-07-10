using System.Collections.Generic;

namespace Delivery.Modelos.DTOs
{
    /// <summary>
    /// Representa el carrito de compras en memoria (sesión del navegador).
    /// NO se persiste en la base de datos hasta que el usuario confirme la compra.
    /// </summary>
    public class CarritoSesionDto
    {
        public long RestauranteId { get; set; }
        public string NombreRestaurante { get; set; } = string.Empty;
        public List<CarritoItemSesionDto> Items { get; set; } = new();
        public string? Notas { get; set; }
        public string? ComprobanteTransferenciaUrl { get; set; }

        public decimal Subtotal
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                    total += item.PrecioUnitario * item.Cantidad;
                return total;
            }
        }
    }

    /// <summary>
    /// Un ítem dentro del carrito de sesión.
    /// </summary>
    public class CarritoItemSesionDto
    {
        public long ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }

    /// <summary>
    /// DTO para confirmar la compra: recibe la dirección y el método de pago.
    /// </summary>
    public class ConfirmarCompraDto
    {
        public long DireccionId { get; set; }
    }
}
