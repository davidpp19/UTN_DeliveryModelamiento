using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("productos")]
    public class Producto
    {
        public Producto() { }

        // Copy constructor
        public Producto(Producto oldProduct)
        {
            if (oldProduct != null)
            {
                this.Id = oldProduct.Id;
                this.RestauranteId = oldProduct.RestauranteId;
                this.CategoriaId = oldProduct.CategoriaId;
                this.Nombre = oldProduct.Nombre;
                this.Descripcion = oldProduct.Descripcion;
                this.Precio = oldProduct.Precio;
                this.ImagenUrl = oldProduct.ImagenUrl;
                this.Disponible = oldProduct.Disponible;
                this.TiempoPreparacion = oldProduct.TiempoPreparacion;
                this.CreadoEn = oldProduct.CreadoEn;
                this.ActualizadoEn = oldProduct.ActualizadoEn;
                this.Stock = oldProduct.Stock;
            }
        }
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("restaurante_id")]
        public long RestauranteId { get; set; }

        [Column("categoria_id")]
        public long? CategoriaId { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Column("descripcion", TypeName = "text")]
        public string? Descripcion { get; set; }

        [Column("precio", TypeName = "numeric(10,2)")]
        public decimal Precio { get; set; }

        [Column("imagen_url", TypeName = "text")]
        public string? ImagenUrl { get; set; }

        [Column("disponible")]
        public bool Disponible { get; set; } = true;

        [Column("tiempo_preparacion")]
        public int? TiempoPreparacion { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        public virtual CategoriaProducto? Categoria { get; set; }

        // ------------------ UML IMPLEMENTATION ------------------
        
        [Column("stock")]
        public int Stock { get; set; } = 0;

        public void UpdateQuantity(int quantity)
        {
            this.Stock += quantity;
            if (this.Stock < 0) this.Stock = 0;
            this.Disponible = this.Stock > 0;
        }
    }
}
