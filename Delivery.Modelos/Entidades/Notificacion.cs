using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Required]
        [Column("titulo")]
        [StringLength(100)]
        public string Titulo { get; set; } = null!;

        [Required]
        [Column("mensaje", TypeName = "text")]
        public string Mensaje { get; set; } = null!;

        [Column("leida")]
        public bool Leida { get; set; } = false;

        [Required]
        [Column("creada_en")]
        public DateTime CreadaEn { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }
    }
}
