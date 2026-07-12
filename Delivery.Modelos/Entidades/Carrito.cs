using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("carritos")]
    public class Carrito
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Required]
        [Column("restaurante_id")]
        public long RestauranteId { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        public virtual ICollection<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}
