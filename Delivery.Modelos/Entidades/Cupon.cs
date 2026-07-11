using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("cupones")]
    public class Cupon
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("codigo")]
        [StringLength(50)]
        public string Codigo { get; set; } = null!;

        [Required]
        [Column("descripcion")]
        [StringLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        [Column("tipo_descuento")]
        public TipoDescuentoEnum TipoDescuento { get; set; }

        [Column("valor_descuento", TypeName = "numeric(10,2)")]
        public decimal ValorDescuento { get; set; }

        [Column("descuento_maximo", TypeName = "numeric(10,2)")]
        public decimal? DescuentoMaximo { get; set; }

        [Column("pedido_minimo", TypeName = "numeric(10,2)")]
        public decimal? PedidoMinimo { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("limite_usos")]
        public int? LimiteUsos { get; set; }

        [Column("usos_actuales")]
        public int UsosActuales { get; set; } = 0;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("es_publico")]
        public bool EsPublico { get; set; } = true;

        [Column("usuario_exclusivo_id")]
        public long? UsuarioExclusivoId { get; set; }

        [Column("restaurante_id")]
        public long? RestauranteId { get; set; }

        [ForeignKey(nameof(UsuarioExclusivoId))]
        public virtual Usuario? UsuarioExclusivo { get; set; }

        [ForeignKey(nameof(RestauranteId))]
        public virtual Restaurante? Restaurante { get; set; }

        public virtual ICollection<CuponUsuario> CuponesUsuarios { get; set; } = new List<CuponUsuario>();
    }
}
