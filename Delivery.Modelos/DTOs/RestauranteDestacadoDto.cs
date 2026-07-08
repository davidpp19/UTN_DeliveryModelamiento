namespace Delivery.Modelos.DTOs
{
    public class RestauranteDestacadoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Abierto { get; set; }
    }
}
