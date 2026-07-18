using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Delivery.Modelos.Interfaces;

namespace Delivery.Modelos.Entidades
{
    [Table("detalle_pedido")]
    public class DetallePedido : ICalculate
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("pedido_id")]
        public long PedidoId { get; set; }

        [Column("producto_id")]
        public long ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario", TypeName = "numeric(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal", TypeName = "numeric(10,2)")]
        public decimal Subtotal { get; set; }

        [Column("notas_especiales")]
        [StringLength(255)]
        public string? NotasEspeciales { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }

        // ------------------ UML IMPLEMENTATION ------------------
        
        public double ICalculateIVA(double valuePay) { return 0; }
        
        public double ICalculateTotal() { return 0; }
        
        public double ICalculateSubtotal()
        {
            return (double)this.Subtotal;
        }
        
        public double ICalculateCurrentTotal() { return 0; }
    }
}
