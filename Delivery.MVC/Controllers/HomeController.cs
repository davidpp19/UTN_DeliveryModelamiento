using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.MVC.Models;
using Delivery.Consumer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Delivery.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRestauranteConsumer _restauranteConsumer;
    private readonly ICategoriaProductoConsumer _categoriaConsumer;
    private readonly ICuponConsumer _cuponConsumer;
    private readonly IProductoConsumer _productoConsumer;

    public HomeController(ILogger<HomeController> logger, IRestauranteConsumer restauranteConsumer, ICategoriaProductoConsumer categoriaConsumer, ICuponConsumer cuponConsumer, IProductoConsumer productoConsumer)
    {
        _logger = logger;
        _restauranteConsumer = restauranteConsumer;
        _categoriaConsumer = categoriaConsumer;
        _cuponConsumer = cuponConsumer;
        _productoConsumer = productoConsumer;
    }

    public async Task<IActionResult> Index()
    {
        var restaurantes = await _restauranteConsumer.GetAllAsync();
        var categorias = await _categoriaConsumer.GetAllAsync();
        var cupones = await _cuponConsumer.GetAllAsync();
        var productos = await _productoConsumer.GetAllAsync();

        var vm = new HomeViewModel
        {
            RestaurantesDestacados = restaurantes?.Take(6) ?? new List<Delivery.Modelos.Entidades.Restaurante>(),
            NuevosRestaurantes = restaurantes?.OrderByDescending(r => r.Id).Take(3) ?? new List<Delivery.Modelos.Entidades.Restaurante>(),
            Categorias = categorias ?? new List<Delivery.Modelos.Entidades.CategoriaProducto>(),
            Cupones = cupones?.Where(c => c.Activo && c.FechaFin >= DateTime.UtcNow).Take(3) ?? new List<Delivery.Modelos.Entidades.Cupon>(),
            ProductosDestacados = productos?.Take(8) ?? new List<Delivery.Modelos.Entidades.Producto>()
        };

        return View(vm);
    }

    public async Task<IActionResult> Restaurante(long id)
    {
        var restaurante = await _restauranteConsumer.GetByIdAsync(id);
        if (restaurante == null) return NotFound();

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
            return View(new { Restaurantes = new List<Delivery.Modelos.Entidades.Restaurante>(), Productos = new List<Delivery.Modelos.Entidades.Producto>() });
        }

        var restaurantes = await _restauranteConsumer.GetAllAsync();
        var productos = await _productoConsumer.GetAllAsync();

        var qLower = q.ToLower();
        var resFiltrados = restaurantes?.Where(r => r.Nombre.ToLower().Contains(qLower) || (r.Categoria != null && r.Categoria.ToLower().Contains(qLower))).ToList();
        var prodFiltrados = productos?.Where(p => p.Nombre.ToLower().Contains(qLower) || (p.Descripcion != null && p.Descripcion.ToLower().Contains(qLower))).ToList();

        return View(new { Restaurantes = resFiltrados, Productos = prodFiltrados });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
