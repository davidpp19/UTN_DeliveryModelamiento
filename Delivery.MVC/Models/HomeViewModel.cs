using System.Collections.Generic;
using Delivery.Modelos.Entidades;

namespace Delivery.MVC.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Restaurante> RestaurantesDestacados { get; set; } = new List<Restaurante>();
        public IEnumerable<Restaurante> NuevosRestaurantes { get; set; } = new List<Restaurante>();
        public IEnumerable<Restaurante> TodosRestaurantes { get; set; } = new List<Restaurante>();
        public IEnumerable<CategoriaProducto> Categorias { get; set; } = new List<CategoriaProducto>();
        public IEnumerable<Cupon> Cupones { get; set; } = new List<Cupon>();
        public IEnumerable<Producto> ProductosDestacados { get; set; } = new List<Producto>();
    }
}

