using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.Entidades
{
    [Table("vehiculos")]
    public class Vehiculo
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("repartidor_id")]
        public long RepartidorId { get; set; }

        [Column("tipo_vehiculo")]
        public TipoVehiculoEnum TipoVehiculo { get; set; }

        [Required]
        [Column("placa")]
        [StringLength(20)]
        public string Placa { get; set; } = null!;

        [Required]
        [Column("marca")]
        [StringLength(50)]
        public string Marca { get; set; } = null!;

        [Required]
        [Column("modelo")]
        [StringLength(50)]
        public string Modelo { get; set; } = null!;

        [Column("anio")]
        public short? Anio { get; set; }

        [Column("color")]
        [StringLength(30)]
        public string? Color { get; set; }

        [Column("foto_vehiculo_url", TypeName = "text")]
        public string? FotoVehiculoUrl { get; set; }

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [ForeignKey(nameof(RepartidorId))]
        public virtual Repartidor? Repartidor { get; set; }
    }
}
