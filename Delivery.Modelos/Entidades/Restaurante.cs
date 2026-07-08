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

        [Column("telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Column("email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Column("logo_url", TypeName = "text")]
        public string? LogoUrl { get; set; }

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
    }
}
