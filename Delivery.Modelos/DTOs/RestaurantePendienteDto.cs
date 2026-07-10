using System;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class RestaurantePendienteDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public string NombresPropietario { get; set; } = string.Empty;
        public string ApellidosPropietario { get; set; } = string.Empty;
        public string EmailPropietario { get; set; } = string.Empty;
    }
}
