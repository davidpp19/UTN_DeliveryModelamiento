using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("registros_auditoria")]
    public class RegistroAuditoria
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("usuario_id")]
        public long? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("modulo_afectado")]
        public string ModuloAfectado { get; set; } = string.Empty;

        [Required]
        [Column("tipo_accion")]
        public TipoAccionAuditoriaEnum TipoAccion { get; set; }

        [Column("detalles")]
        public string? Detalles { get; set; }
    }
}
