using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Column("descripcion")]
        [StringLength(255)]
        public string? Descripcion { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
