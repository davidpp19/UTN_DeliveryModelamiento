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
        [StringLength(20)]
        public string Cedula { get; set; } = null!;

        [Required(ErrorMessage = "El nombre del restaurante es obligatorio")]
        public string NombreRestaurante { get; set; } = null!;

        [Required(ErrorMessage = "El RUC es obligatorio")]
        public string RUC { get; set; } = null!; // Lo podemos guardar temporalmente en teléfono u otra tabla si no existe RUC en Usuario/Restaurante (Wait, Restaurante no tiene RUC? I will use 'Descripcion' or I should add 'RUC' to Restaurante, let me just add it to the DTO and I can map it). Actually I'll see if I need a migration for RUC later.

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "La dirección (calle) es obligatoria")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        public string Ciudad { get; set; } = null!;

        public string? Categoria { get; set; }
        
        public string? Descripcion { get; set; }

        public TimeSpan? HoraApertura { get; set; }
        public TimeSpan? HoraCierre { get; set; }
    }
}
