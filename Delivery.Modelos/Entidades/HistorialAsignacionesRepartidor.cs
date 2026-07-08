using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("historial_asignaciones_repartidor")]
    public class HistorialAsignacionesRepartidor
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("repartidor_id")]
        public long RepartidorId { get; set; }

        [Column("pedido_id")]
        public long PedidoId { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [ForeignKey(nameof(RepartidorId))]
        public virtual Repartidor? Repartidor { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public virtual Pedido? Pedido { get; set; }
    }
}
