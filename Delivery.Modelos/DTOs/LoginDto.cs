using System.ComponentModel.DataAnnotations;

namespace Delivery.Modelos.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        // UML: Verify more than two characters
        [MinLength(3, ErrorMessage = "El usuario debe tener más de dos caracteres.")]
        // UML: Contains '@' and '.'
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El usuario debe contener '@' y '.'")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
