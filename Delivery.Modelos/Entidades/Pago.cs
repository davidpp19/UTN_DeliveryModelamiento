using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("pedido_id")]
        public long PedidoId { get; set; }

        [Column("tipo_metodo_pago")]
        public TipoMetodoPagoEnum TipoMetodoPago { get; set; }

        [Column("monto", TypeName = "numeric(10,2)")]
        public decimal Monto { get; set; }

        [Column("estado_pago")]
        public EstadoPagoEnum EstadoPago { get; set; }

        [Column("referencia_transaccion")]
        [StringLength(100)]
        public string? ReferenciaTransaccion { get; set; }

        [Column("fecha_pago")]
        public DateTime? FechaPago { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public virtual Pedido? Pedido { get; set; }
    }
}
