using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("carrito_items")]
    public class CarritoItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("carrito_id")]
        public long CarritoId { get; set; }

        [Required]
        [Column("producto_id")]
        public long ProductoId { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "numeric(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("notas")]
        [StringLength(500)]
        public string? Notas { get; set; }

        [ForeignKey(nameof(CarritoId))]
        public virtual Carrito? Carrito { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }
    }
}
