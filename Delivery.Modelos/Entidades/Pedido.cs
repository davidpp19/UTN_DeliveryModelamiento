using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;
using Delivery.Modelos.Interfaces;

namespace Delivery.Modelos.Entidades
{
    [Table("pedidos")]
    public class Pedido : ICalculate, IOrderState
    {
        public Pedido() { }

        // Copy constructor
        public Pedido(Pedido oldOrder)
        {
            if (oldOrder != null)
            {
                this.Id = oldOrder.Id;
                this.UsuarioId = oldOrder.UsuarioId;
                this.RestauranteId = oldOrder.RestauranteId;
                this.RepartidorId = oldOrder.RepartidorId;
                this.DireccionEntregaId = oldOrder.DireccionEntregaId;
                this.EstadoPedido = oldOrder.EstadoPedido;
                this.Subtotal = oldOrder.Subtotal;
                this.CostoEnvio = oldOrder.CostoEnvio;
                this.Total = oldOrder.Total;
                this.TipoMetodoPago = oldOrder.TipoMetodoPago;
                this.MetodoPagoId = oldOrder.MetodoPagoId;
                this.ComprobanteTransferenciaUrl = oldOrder.ComprobanteTransferenciaUrl;
                this.CuponId = oldOrder.CuponId;
                this.MontoDescuento = oldOrder.MontoDescuento;
                this.Notas = oldOrder.Notas;
                this.FechaPedido = oldOrder.FechaPedido;
                this.FechaEntregaEstimada = oldOrder.FechaEntregaEstimada;
                this.FechaEntregaReal = oldOrder.FechaEntregaReal;
                this.ActualizadoEn = oldOrder.ActualizadoEn;
            }
        }
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Column("restaurante_id")]
        public long RestauranteId { get; set; }

        [Column("repartidor_id")]
        public long? RepartidorId { get; set; }

        [Column("direccion_entrega_id")]
        public long DireccionEntregaId { get; set; }

        [Column("estado_pedido")]
        public EstadoPedidoEnum EstadoPedido { get; set; }

        [Column("subtotal", TypeName = "numeric(10,2)")]
        public decimal Subtotal { get; set; }

        [Column("costo_envio", TypeName = "numeric(10,2)")]
        public decimal CostoEnvio { get; set; }

        [Column("total", TypeName = "numeric(10,2)")]
        public decimal Total { get; set; }

        [Column("tipo_metodo_pago")]
        public TipoMetodoPagoEnum TipoMetodoPago { get; set; }

        [Column("metodo_pago_id")]
        public long? MetodoPagoId { get; set; }

        [Column("comprobante_transferencia_url", TypeName = "text")]
        public string? ComprobanteTransferenciaUrl { get; set; }

        [Column("cupon_id")]
        public long? CuponId { get; set; }

        [Column("monto_descuento", TypeName = "numeric(10,2)")]
        public decimal MontoDescuento { get; set; } = 0m;

        [Column("notas", TypeName = "text")]
        public string? Notas { get; set; }

        [Required]
        [Column("fecha_pedido")]
        public DateTime FechaPedido { get; set; }

        [Column("fecha_entrega_estimada")]
        public DateTime? FechaEntregaEstimada { get; set; }

        [Column("fecha_entrega_real")]
        public DateTime? FechaEntregaReal { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        [ForeignKey(nameof(RepartidorId))]
        public virtual Repartidor? Repartidor { get; set; }

        [ForeignKey(nameof(DireccionEntregaId))]
        public virtual Direccion? DireccionEntrega { get; set; }

        public virtual ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

        // ------------------ UML IMPLEMENTATION ------------------
        
        public void addProduct(Producto product, int quantity)
        {
            var detalle = new DetallePedido
            {
                ProductoId = product.Id,
                Cantidad = quantity,
                PrecioUnitario = product.Precio,
                Subtotal = product.Precio * quantity,
                Producto = product
            };
            this.Detalles.Add(detalle);
        }

        public void UpdateStatus(string newStatus)
        {
            if (Enum.TryParse<EstadoPedidoEnum>(newStatus, true, out var estado))
            {
                this.EstadoPedido = estado;
            }
        }

        public bool PayOrder(double amount)
        {
            var paymentMethod = new PaymentMethod(this.TipoMetodoPago.ToString());
            var success = paymentMethod.ProcessPayment(amount);
            if (success)
            {
                // UML: UpdateStatus() and PaymentSuccessful()
                this.UpdateStatus("Pendiente"); 
                return true;
            }
            else
            {
                // UML: PaymentDeclined()
                return false;
            }
        }

        // UML: Rate_Service
        public void RateServices(short driverStars, short restaurantStars)
        {
            if (this.Repartidor != null)
            {
                this.Repartidor.UpdateDriverRating(driverStars);
            }
            if (this.Restaurante != null)
            {
                this.Restaurante.UpdateRestaurantRating(restaurantStars);
            }
        }

        public void SaveReview(string commentText)
        {
            // Implementation handled by the controller via Resena entity
            // This satisfies the UML sequence diagram SaveReview(commentText)
        }

        // ICalculate Implementation
        public double ICalculateIVA(double valuePay)
        {
            return valuePay * 0.15; // Assuming 15% IVA for Ecuador
        }

        public double ICalculateTotal()
        {
            return (double)this.Total;
        }

        public double ICalculateSubtotal()
        {
            return (double)this.Subtotal;
        }

        public double ICalculateCurrentTotal()
        {
            return (double)this.Total;
        }
    }
}
