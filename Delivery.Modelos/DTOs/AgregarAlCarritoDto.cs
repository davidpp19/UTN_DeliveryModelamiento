using System.ComponentModel.DataAnnotations;

namespace Delivery.Modelos.DTOs
{
    public class AgregarAlCarritoDto
    {
        [Required]
        public long UsuarioId { get; set; }
        
        [Required]
        public long RestauranteId { get; set; }
        
        [Required]
        public long ProductoId { get; set; }
        
        [Range(1, 100)]
        public int Cantidad { get; set; } = 1;
        
        public string? Notas { get; set; }
    }
}
