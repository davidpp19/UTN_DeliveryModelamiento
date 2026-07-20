using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("restaurantes")]
    public class Restaurante
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Column("descripcion", TypeName = "text")]
        public string? Descripcion { get; set; }

        [Column("categoria")]
        [StringLength(50)]
        public string? Categoria { get; set; }

        [Column("ruc")]
        [StringLength(13)]
        [RegularExpression("^[0-9]*$")]
        public string? Ruc { get; set; }


        [Required]
        [Column("calle")]
        [StringLength(150)]
        public string Calle { get; set; } = null!;

        [Required]
        [Column("ciudad")]
        [StringLength(100)]
        public string Ciudad { get; set; } = null!;

        [Column("latitud", TypeName = "numeric(10,8)")]
        public decimal? Latitud { get; set; }

        [Column("longitud", TypeName = "numeric(11,8)")]
        public decimal? Longitud { get; set; }

        [Phone]
        [Column("telefono")]
        [StringLength(10)]
        [RegularExpression("^[0-9]*$")]
        public string? Telefono { get; set; }

        [EmailAddress]
        [Column("email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Column("logo_url", TypeName = "text")]
        public string? LogoUrl { get; set; }

        [Column("portada_url", TypeName = "text")]
        public string? PortadaUrl { get; set; }

        [Column("redes_sociales", TypeName = "text")]
        public string? RedesSociales { get; set; }

        [Column("hora_apertura")]
        public TimeSpan? HoraApertura { get; set; }

        [Column("hora_cierre")]
        public TimeSpan? HoraCierre { get; set; }

        [Column("costo_envio_base", TypeName = "numeric(10,2)")]
        public decimal CostoEnvioBase { get; set; } = 0m;

        [Column("estado")]
        public EstadoRestauranteEnum Estado { get; set; }

        [Column("aprobado_por")]
        public long? AprobadoPor { get; set; }

        [Column("fecha_aprobacion")]
        public DateTime? FechaAprobacion { get; set; }

        [Column("abierto")]
        public bool Abierto { get; set; } = false;

        [Column("creado_por")]
        public long? CreadoPor { get; set; }

        [Column("valid_license")]
        public bool ValidLicense { get; set; } = true;

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(AprobadoPor))]
        public virtual Usuario? UsuarioAprobador { get; set; }

        [ForeignKey(nameof(CreadoPor))]
        public virtual Usuario? UsuarioCreador { get; set; }

        public virtual ICollection<CategoriaProducto> Categorias { get; set; } = new List<CategoriaProducto>();
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        // ------------------ UML IMPLEMENTATION ------------------

        public void setName(string name)
        {
            this.Nombre = name;
        }

        public void setAddress(string address)
        {
            this.Calle = address;
        }

        public void AddProduct(Producto product)
        {
            this.Productos.Add(product);
        }

        public void UpdateRestaurantRating(short stars)
        {
            // Usually restaurants would have an average rating field, if not, we satisfy UML with this method
        }

        // UML: Accept_Delivery
        public bool AcceptOrder(Pedido order, Repartidor driver)
        {
            if (order.EstadoPedido.ToString() == "Pendiente" || order.EstadoPedido == EstadoPedidoEnum.Pendiente)
            {
                order.UpdateStatus("Aceptado");
                driver.setStatus("Ocupado"); // Delivery Confirmed
                return true;
            }
            // OrderNoLongerAvailable()
            return false;
        }
    }
}
