using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("repartidores")]
    public class Repartidor
    {
        public Repartidor() { }

        // Copy constructor
        public Repartidor(Repartidor oldDriver)
        {
            if (oldDriver != null)
            {
                this.UsuarioId = oldDriver.UsuarioId;
                this.LicenciaConducir = oldDriver.LicenciaConducir;
                this.FotoLicenciaUrl = oldDriver.FotoLicenciaUrl;
                this.EstadoAprobacion = oldDriver.EstadoAprobacion;
                this.Direccion = oldDriver.Direccion;
                this.DatosAdicionales = oldDriver.DatosAdicionales;
                this.AprobadoPor = oldDriver.AprobadoPor;
                this.FechaAprobacion = oldDriver.FechaAprobacion;
                this.Estado = oldDriver.Estado;
                this.CalificacionPromedio = oldDriver.CalificacionPromedio;
                this.CreadoEn = oldDriver.CreadoEn;
                this.ActualizadoEn = oldDriver.ActualizadoEn;
                this.Comission = oldDriver.Comission;
            }
        }
        [Key]
        [Column("usuario_id")]
        public long UsuarioId { get; set; }

        [Required]
        [Column("licencia_conducir")]
        [StringLength(50)]
        public string LicenciaConducir { get; set; } = null!;

        [Column("foto_licencia_url", TypeName = "text")]
        public string? FotoLicenciaUrl { get; set; }


        [Column("estado_aprobacion")]
        public EstadoAprobacionEnum EstadoAprobacion { get; set; }

        [Column("direccion", TypeName = "text")]
        public string? Direccion { get; set; }

        [Column("datos_adicionales", TypeName = "text")]
        public string? DatosAdicionales { get; set; }

        [Column("aprobado_por")]
        public long? AprobadoPor { get; set; }

        [Column("fecha_aprobacion")]
        public DateTime? FechaAprobacion { get; set; }

        [Column("estado")]
        public EstadoRepartidorEnum Estado { get; set; } = EstadoRepartidorEnum.Desconectado;

        [Column("calificacion_promedio", TypeName = "numeric(3,2)")]
        public decimal? CalificacionPromedio { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("actualizado_en")]
        public DateTime? ActualizadoEn { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(AprobadoPor))]
        public virtual Usuario? UsuarioAprobador { get; set; }

        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public virtual UbicacionActualRepartidor? UbicacionActual { get; set; }
        public virtual ICollection<HistorialAsignacionesRepartidor> HistorialAsignaciones { get; set; } = new List<HistorialAsignacionesRepartidor>();

        // ------------------ UML IMPLEMENTATION ------------------
        
        [Column("comission")]
        public double? Comission { get; set; }

        public double CalculateComission(double orderTotal)
        {
            // By default 10% or whatever is stored
            return orderTotal * (Comission ?? 0.10);
        }

        public void setStatus(string status)
        {
            if (Enum.TryParse<EstadoRepartidorEnum>(status, true, out var estado))
            {
                this.Estado = estado;
            }
        }

        public void UpdateDriverRating(short stars)
        {
            // Calculate new average or simply assign
            if (this.CalificacionPromedio == null) this.CalificacionPromedio = stars;
            else this.CalificacionPromedio = (this.CalificacionPromedio + stars) / 2m;
        }
    }
}
