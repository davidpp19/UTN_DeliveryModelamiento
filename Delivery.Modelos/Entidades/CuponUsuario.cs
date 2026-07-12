using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("cupones_usuarios")]
    public class CuponUsuario
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("cupon_id")]
        public long CuponId { get; set; }

        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Column("pedido_id")]
        public long? PedidoId { get; set; }

        [Column("usado")]
        public bool Usado { get; set; } = false;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_asignacion")]
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_expiracion")]
        public DateTime? FechaExpiracion { get; set; }

        [Column("fecha_uso")]
        public DateTime? FechaUso { get; set; }

        [ForeignKey(nameof(CuponId))]
        public virtual Cupon? Cupon { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public virtual Pedido? Pedido { get; set; }
    }
}
