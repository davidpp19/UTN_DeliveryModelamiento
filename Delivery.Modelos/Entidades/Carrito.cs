using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Delivery.Modelos.Interfaces;
using System.Linq;

namespace Delivery.Modelos.Entidades
{
    [Table("carritos")]
    public class Carrito : ICalculate
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

        // ------------------ UML IMPLEMENTATION ------------------
        
        [NotMapped]
        public double CurrentTotal { get; set; }

        public void AddProduct(Producto product, int quantity)
        {
            var item = this.Items.FirstOrDefault(i => i.ProductoId == product.Id);
            if (item != null)
            {
                item.Cantidad += quantity;
            }
            else
            {
                this.Items.Add(new CarritoItem { ProductoId = product.Id, Producto = product, Cantidad = quantity });
            }
        }

        public void RemoveProduct(Producto product)
        {
            var item = this.Items.FirstOrDefault(i => i.ProductoId == product.Id);
            if (item != null)
            {
                this.Items.Remove(item);
            }
        }

        public void ClearCart()
        {
            this.Items.Clear();
        }

        public Pedido Checkout()
        {
            // Placeholder: This is handled by the service layer, but UML requires it here
            return new Pedido();
        }

        // ICalculate Implementation
        public double ICalculateIVA(double valuePay) { return 0; }
        public double ICalculateTotal() { return 0; }
        public double ICalculateSubtotal() { return 0; }
        public double ICalculateCurrentTotal()
        {
            this.CurrentTotal = this.Items.Sum(i => (double)(i.Producto?.Precio ?? 0) * i.Cantidad);
            return this.CurrentTotal;
        }
    }
}
