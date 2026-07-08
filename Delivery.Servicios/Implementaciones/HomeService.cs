using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class HomeService : IHomeService
    {
        private readonly DeliveryDbContext _context;

        public HomeService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<HomeResponseDto> ObtenerDatosHomeAsync()
        {
            // Categorias Destacadas (Podríamos tomar las que más productos tienen, o hardcoded)
            var categorias = await _context.CategoriasProducto
                                           .Select(c => c.Nombre)
                                           .Distinct()
                                           .Take(10)
                                           .ToListAsync();

            // Restaurantes Mejor Calificados
            // Dado que no hay un campo CalificacionPromedio en Restaurante (solo Resenas),
            // podemos calcularlo al vuelo o usar un dummy si no queremos complicar la consulta
            // Aquí lo simularemos asumiendo que un restaurante tiene reseñas
            
            var restaurantesTop = await _context.Restaurantes
                .Select(r => new RestauranteDestacadoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Categoria = r.Categoria,
                    Abierto = r.Abierto,
                    ImagenUrl = r.LogoUrl,
                    CalificacionPromedio = _context.Resenas
                                                   .Where(res => res.RestauranteId == r.Id)
                                                   .Average(res => (decimal?)res.CalificacionRestaurante) ?? 5.0m
                })
                .OrderByDescending(r => r.CalificacionPromedio)
                .Take(5)
                .ToListAsync();

            var promociones = new List<string>
            {
                "¡Envío gratis en tu primer pedido!",
                "20% de descuento en Pizzas los viernes"
            };

            return new HomeResponseDto
            {
                CategoriasDestacadas = categorias,
                RestaurantesMejorCalificados = restaurantesTop,
                PromocionesActivas = promociones
            };
        }
    }
}
