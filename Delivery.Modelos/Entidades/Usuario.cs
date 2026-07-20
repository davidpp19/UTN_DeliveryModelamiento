using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("usuarios")]
    public class Usuario : Person
    {
        public Usuario() : base() { }

        // Copy constructor
        public Usuario(Usuario oldUser) : base(oldUser)
        {
            if (oldUser != null)
            {
                this.Id = oldUser.Id;
                this.RolId = oldUser.RolId;
                this.Cedula = oldUser.Cedula;
                this.Nombre = oldUser.Nombre;
                this.Apellidos = oldUser.Apellidos;
                this.Email = oldUser.Email;
                this.PasswordHash = oldUser.PasswordHash;
                this.Telefono = oldUser.Telefono;
                this.TipoUsuario = oldUser.TipoUsuario;
                this.Activo = oldUser.Activo;
                this.FotoPerfilUrl = oldUser.FotoPerfilUrl;
                this.FechaNacimiento = oldUser.FechaNacimiento;
                this.InformacionAdicional = oldUser.InformacionAdicional;
                this.EmailConfirmado = oldUser.EmailConfirmado;
                this.CodigoVerificacion = oldUser.CodigoVerificacion;
                this.ExpiracionCodigo = oldUser.ExpiracionCodigo;
                this.CreadoEn = oldUser.CreadoEn;
                this.ActualizadoEn = oldUser.ActualizadoEn;
                this.Branch = oldUser.Branch;
                this.Salary = oldUser.Salary;
            }
        }
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("rol_id")]
        public long? RolId { get; set; }

        [Required]
        [Column("cedula")]
        [StringLength(10)]
        [RegularExpression("^[0-9]*$")]
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
        [StringLength(10)]
        [RegularExpression("^[0-9]*$")]
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

        [Column("intentos_fallidos")]
        public int IntentosFallidos { get; set; } = 0;

        [Column("bloqueado_hasta")]
        public DateTime? BloqueadoHasta { get; set; }

        [ForeignKey(nameof(RolId))]
        public virtual Rol? Rol { get; set; }

        public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();

        // ------------------ UML IMPLEMENTATION ------------------
        
        [Column("branch")]
        [StringLength(100)]
        public string? Branch { get; set; } // For Admin

        [Column("salary")]
        public double? Salary { get; set; } // For Admin

        // Mapping properties to abstract Person
        [NotMapped]
        public override string Dni { get => Cedula; set => Cedula = value; }

        [NotMapped]
        public override string Name { get => $"{Nombre} {Apellidos}"; set { /* Handled separately */ } }

        [NotMapped]
        public override Fecha DateBirth 
        { 
            get => FechaNacimiento.HasValue ? Fecha.FromDateTime(FechaNacimiento.Value) : new Fecha(); 
            set => FechaNacimiento = value?.ToDateTime(); 
        }

        [NotMapped]
        public override string Mail { get => Email; set => Email = value; }

        [NotMapped]
        public override string Phone { get => Telefono ?? ""; set => Telefono = value; }

        [NotMapped]
        public override string Password { get => PasswordHash; set => PasswordHash = value; }

        [NotMapped]
        public override string Address 
        { 
            get => string.Empty; // Needs to pull from Direcciones if requested, not heavily used directly 
            set { } 
        }

        // --- Client Methods ---
        public int CalculateNumberOrders()
        {
            // Placeholder: implement in service
            return 0;
        }

        public int RankServices()
        {
            // Placeholder: implement in service
            return 0;
        }

        public double Pay(double value)
        {
            return value;
        }

        // --- Admin Methods ---
        public string ViewListClient()
        {
            return "List of clients";
        }

        public string ViewListDelivery()
        {
            return "List of drivers";
        }

        public void AddPerson() { }

        public bool ApproveDelivery(Repartidor detailsVehicle)
        {
            return true;
        }

        public string generateCode(string mail)
        {
            return "CODE123";
        }

        public bool verifyMail(string mail)
        {
            return true;
        }
    }
}
