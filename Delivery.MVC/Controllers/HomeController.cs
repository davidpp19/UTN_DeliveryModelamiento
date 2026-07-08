using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Delivery.MVC.Models;
using Delivery.Consumer.Interfaces;
using Delivery.Modelos.Entidades;

namespace Delivery.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRestauranteConsumer _restauranteConsumer;
    private readonly ICategoriaProductoConsumer _categoriaConsumer;
    private readonly ICuponConsumer _cuponConsumer;
    private readonly IProductoConsumer _productoConsumer;

    // Imágenes públicas estables por categoría de comida
    private static readonly Dictionary<string, string> _imagenesPorCategoria = new(StringComparer.OrdinalIgnoreCase)
    {
        { "pizza",        "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80" },
        { "hamburguesa",  "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600&q=80" },
        { "hamburguesas", "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600&q=80" },
        { "pollo",        "https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?w=600&q=80" },
        { "sushi",        "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?w=600&q=80" },
        { "parrillada",   "https://images.unsplash.com/photo-1558030006-450675393462?w=600&q=80" },
        { "parrilladas",  "https://images.unsplash.com/photo-1558030006-450675393462?w=600&q=80" },
        { "comida típica","https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&q=80" },
        { "comida tipica","https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&q=80" },
        { "cafetería",    "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=600&q=80" },
        { "cafeteria",    "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=600&q=80" },
        { "postres",      "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=600&q=80" },
        { "postre",       "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=600&q=80" },
        { "tacos",        "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?w=600&q=80" },
        { "mariscos",     "https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=600&q=80" },
        { "ensaladas",    "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600&q=80" },
        { "pasta",        "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=600&q=80" },
        { "sandwiches",   "https://images.unsplash.com/photo-1553909489-cd47e0907980?w=600&q=80" },
        { "sandwich",     "https://images.unsplash.com/photo-1553909489-cd47e0907980?w=600&q=80" },
    };

    private const string _imagenDefault = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=600&q=80";

    public static string ObtenerImagenRestaurante(Restaurante res)
    {
        // 1. Usar logoUrl si ya existe y no está vacío
        if (!string.IsNullOrWhiteSpace(res.LogoUrl) && res.LogoUrl.StartsWith("http"))
            return res.LogoUrl;

        // 2. Intentar por categoría
        if (!string.IsNullOrWhiteSpace(res.Categoria))
        {
            var catLower = res.Categoria.ToLower().Trim();
            foreach (var kv in _imagenesPorCategoria)
            {
                if (catLower.Contains(kv.Key))
                    return kv.Value;
            }
        }

        // 3. Imagen genérica de restaurante
        return _imagenDefault;
    }

    public HomeController(
        ILogger<HomeController> logger,
        IRestauranteConsumer restauranteConsumer,
        ICategoriaProductoConsumer categoriaConsumer,
        ICuponConsumer cuponConsumer,
        IProductoConsumer productoConsumer)
    {
        _logger = logger;
        _restauranteConsumer = restauranteConsumer;
        _categoriaConsumer = categoriaConsumer;
        _cuponConsumer = cuponConsumer;
        _productoConsumer = productoConsumer;
    }

    public async Task<IActionResult> Index()
    {
        var restaurantes = (await _restauranteConsumer.GetAllAsync())?.ToList() ?? new List<Restaurante>();
        var categorias   = await _categoriaConsumer.GetAllAsync();
        var cupones      = await _cuponConsumer.GetAllAsync();
        var productos    = await _productoConsumer.GetAllAsync();

        // Asignar imagen a cada restaurante que no tenga logoUrl
        foreach (var r in restaurantes)
        {
            if (string.IsNullOrWhiteSpace(r.LogoUrl) || !r.LogoUrl.StartsWith("http"))
                r.LogoUrl = ObtenerImagenRestaurante(r);
        }

        var restaurantesActivos = restaurantes.Where(r => r.Abierto).ToList();

        var vm = new HomeViewModel
        {
            // Todos los restaurantes activos para la vista pública
            TodosRestaurantes      = restaurantesActivos,
            // Destacados para el cliente (los primeros 6 activos)
            RestaurantesDestacados = restaurantesActivos.Take(6).ToList(),
            // Nuevos para la sección pública (los 3 más recientes)
            NuevosRestaurantes     = restaurantes.OrderByDescending(r => r.Id).Take(3).ToList(),
            Categorias             = categorias ?? new List<CategoriaProducto>(),
            Cupones                = cupones?.Where(c => c.Activo && c.FechaFin >= DateTime.UtcNow).Take(3) ?? new List<Cupon>(),
            ProductosDestacados    = productos?.Take(8) ?? new List<Producto>()
        };

        return View(vm);
    }

    public async Task<IActionResult> Restaurante(long id)
    {
        var restaurante = await _restauranteConsumer.GetByIdAsync(id);
        if (restaurante == null) return NotFound();

        if (string.IsNullOrWhiteSpace(restaurante.LogoUrl) || !restaurante.LogoUrl.StartsWith("http"))
            restaurante.LogoUrl = ObtenerImagenRestaurante(restaurante);

        var todosProductos = await _productoConsumer.GetAllAsync();
        var productos = todosProductos.Where(p => p.RestauranteId == id && p.Disponible).ToList();

        ViewBag.Productos = productos;
        return View(restaurante);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Buscar(string q)
    {
        ViewBag.Query = q;
        if (string.IsNullOrWhiteSpace(q))
        {
            return View(new { Restaurantes = new List<Restaurante>(), Productos = new List<Producto>() });
        }

        var restaurantes = await _restauranteConsumer.GetAllAsync();
        var productos    = await _productoConsumer.GetAllAsync();

        var qLower = q.ToLower();
        var resFiltrados  = restaurantes?.Where(r =>
            r.Nombre.ToLower().Contains(qLower) ||
            (r.Categoria != null && r.Categoria.ToLower().Contains(qLower))).ToList();
        var prodFiltrados = productos?.Where(p =>
            p.Nombre.ToLower().Contains(qLower) ||
            (p.Descripcion != null && p.Descripcion.ToLower().Contains(qLower))).ToList();

        return View(new { Restaurantes = resFiltrados, Productos = prodFiltrados });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
