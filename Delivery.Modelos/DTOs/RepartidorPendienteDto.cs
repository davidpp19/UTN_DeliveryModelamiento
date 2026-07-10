using System;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class RepartidorPendienteDto
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string LicenciaConducir { get; set; } = string.Empty;
        public string? FotoLicenciaUrl { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public DateTime CreadoEn { get; set; }
        public string TipoVehiculo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
    }
}
