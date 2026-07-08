using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("ubicacion_actual_repartidor")]
    public class UbicacionActualRepartidor
    {
        [Key]
        [Column("repartidor_id")]
        public long RepartidorId { get; set; }

        [Column("latitud", TypeName = "numeric(10,8)")]
        public decimal? Latitud { get; set; }

        [Column("longitud", TypeName = "numeric(11,8)")]
        public decimal? Longitud { get; set; }

        [Column("rumbo", TypeName = "numeric(6,2)")]
        public decimal? Rumbo { get; set; }

        [Column("velocidad", TypeName = "numeric(6,2)")]
        public decimal? Velocidad { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(RepartidorId))]
        public virtual Repartidor? Repartidor { get; set; }
    }
}
