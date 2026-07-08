using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("resenas")]
    public class Resena
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("pedido_id")]
        public long PedidoId { get; set; }

        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Column("restaurante_id")]
        public long? RestauranteId { get; set; }

        [Column("repartidor_id")]
        public long? RepartidorId { get; set; }

        [Column("calificacion_restaurante")]
        [Range(1, 5)]
        public short? CalificacionRestaurante { get; set; }

        [Column("comentario_restaurante", TypeName = "text")]
        public string? ComentarioRestaurante { get; set; }

        [Column("calificacion_repartidor")]
        [Range(1, 5)]
        public short? CalificacionRepartidor { get; set; }

        [Column("comentario_repartidor", TypeName = "text")]
        public string? ComentarioRepartidor { get; set; }

        [Required]
        [Column("fecha_resena")]
        public DateTime FechaResena { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        [ForeignKey(nameof(RepartidorId))]
        public virtual Repartidor? Repartidor { get; set; }
    }
}
