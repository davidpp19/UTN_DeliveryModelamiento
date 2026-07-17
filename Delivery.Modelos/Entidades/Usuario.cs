using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("rol_id")]
        public long? RolId { get; set; }

        [Required]
        [Column("cedula")]
        [StringLength(20)]
        public string Cedula { get; set; } = null!;

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [Column("apellidos")]
        [StringLength(100)]
        public string Apellidos { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Column("email")]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Phone]
        [Column("telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Column("tipo_usuario")]
        public TipoUsuarioEnum TipoUsuario { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("foto_perfil_url")]
        public string? FotoPerfilUrl { get; set; }

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("informacion_adicional", TypeName = "text")]
        public string? InformacionAdicional { get; set; }

        [Column("email_confirmado")]
        public bool EmailConfirmado { get; set; } = false;

        [Column("codigo_verificacion")]
        [StringLength(10)]
        public string? CodigoVerificacion { get; set; }

        [Column("expiracion_codigo")]
        public DateTime? ExpiracionCodigo { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(RolId))]
        public virtual Rol? Rol { get; set; }

        public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    }
}
