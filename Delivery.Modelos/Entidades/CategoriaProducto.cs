using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("categorias_producto")]
    public class CategoriaProducto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("restaurante_id")]
        public long RestauranteId { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Column("descripcion", TypeName = "text")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
