using System.Collections.Generic;

namespace Delivery.Modelos.DTOs
{
    public class HomeResponseDto
    {
        public IEnumerable<string> CategoriasDestacadas { get; set; } = new List<string>();
        public IEnumerable<RestauranteDestacadoDto> RestaurantesMejorCalificados { get; set; } = new List<RestauranteDestacadoDto>();
        // Promociones podría ser una lista de strings o una entidad Promocion
        public IEnumerable<string> PromocionesActivas { get; set; } = new List<string>();
    }
}
