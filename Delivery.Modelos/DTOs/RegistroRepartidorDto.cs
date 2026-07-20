using System.ComponentModel.DataAnnotations;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class RegistroRepartidorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme su contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten números")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es requerida")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten números")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de licencia es requerido")]
        [StringLength(50)]
        public string LicenciaConducir { get; set; } = string.Empty;

        // Datos del Vehículo
        [Required(ErrorMessage = "El tipo de vehículo es requerido")]
        public TipoVehiculoEnum TipoVehiculo { get; set; }

        [Required(ErrorMessage = "La placa es requerida")]
        [StringLength(20)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es requerido")]
        [StringLength(30)]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es requerida")]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es requerido")]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        public short? Anio { get; set; }

        public string? FotoLicenciaBase64 { get; set; }
    }
}
