using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("favoritos")]
    public class Favorito
    {
        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Column("restaurante_id")]
        public long RestauranteId { get; set; }

        [Required]
        [Column("fecha_agregado")]
        public DateTime FechaAgregado { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }
    }
}
