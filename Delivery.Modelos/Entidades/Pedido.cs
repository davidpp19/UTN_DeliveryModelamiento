using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("pedidos")]
    public class Pedido
    {
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
    }
}
