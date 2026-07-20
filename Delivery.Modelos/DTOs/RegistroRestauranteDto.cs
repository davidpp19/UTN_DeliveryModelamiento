using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Modelos.DTOs
{
    public class RegistroRestauranteDto
    {
        [Required(ErrorMessage = "El nombre del propietario es obligatorio")]
        public string NombrePropietario { get; set; } = null!;

        [Required(ErrorMessage = "Los apellidos del propietario son obligatorios")]
        public string ApellidosPropietario { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = null!;

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten números")]
        public string Cedula { get; set; } = null!;

        [Required(ErrorMessage = "El nombre del restaurante es obligatorio")]
        public string NombreRestaurante { get; set; } = null!;

        [Required(ErrorMessage = "El RUC es obligatorio")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El RUC debe tener 13 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten números")]
        public string RUC { get; set; } = null!;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten números")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "La dirección (calle) es obligatoria")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        public string Ciudad { get; set; } = null!;

        [Required(ErrorMessage = "Seleccione su ubicación en el mapa")]
        public decimal? Latitud { get; set; }

        [Required(ErrorMessage = "Seleccione su ubicación en el mapa")]
        public decimal? Longitud { get; set; }

        public string? Categoria { get; set; }
        
        public string? Descripcion { get; set; }

        public TimeSpan? HoraApertura { get; set; }
        public TimeSpan? HoraCierre { get; set; }
    }
}
