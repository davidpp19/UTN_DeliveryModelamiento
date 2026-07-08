using Delivery.Modelos.Enums;

namespace Delivery.Modelos.DTOs
{
    public class FiltrosBusquedaDto
    {
        public string? TerminoBusqueda { get; set; }
        
        // Filtros específicos
        public string? Categoria { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public bool? Disponibilidad { get; set; }
        public EstadoPedidoEnum? EstadoPedido { get; set; }
        public long? RestauranteId { get; set; }
        
        // Paginación base
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 20;
    }
}
