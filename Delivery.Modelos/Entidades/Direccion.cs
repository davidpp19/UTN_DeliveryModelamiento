using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("direcciones")]
    public class Direccion
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Column("alias")]
        [StringLength(50)]
        public string? Alias { get; set; }

        [Required]
        [Column("calle")]
        [StringLength(150)]
        public string Calle { get; set; } = null!;

        [Column("numero")]
        [StringLength(20)]
        public string? Numero { get; set; }

        [Required]
        [Column("ciudad")]
        [StringLength(100)]
        public string Ciudad { get; set; } = null!;

        [Column("referencia")]
        [StringLength(255)]
        public string? Referencia { get; set; }

        [Column("latitud", TypeName = "numeric(10,8)")]
        public decimal? Latitud { get; set; }

        [Column("longitud", TypeName = "numeric(11,8)")]
        public decimal? Longitud { get; set; }

        [Column("es_principal")]
        public bool EsPrincipal { get; set; } = false;

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }
    }
}
