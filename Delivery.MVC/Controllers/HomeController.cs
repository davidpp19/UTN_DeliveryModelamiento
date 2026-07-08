using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.MVC.Models;
using Delivery.Consumer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Delivery.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IRestauranteConsumer _restauranteConsumer;
    private readonly IProductoConsumer _productoConsumer;

    public HomeController(IRestauranteConsumer restauranteConsumer, IProductoConsumer productoConsumer)
    {
        _restauranteConsumer = restauranteConsumer;
        _productoConsumer = productoConsumer;
    }

    public async Task<IActionResult> Index()
    {
        // En el Home mostramos los restaurantes que están activos
        var restaurantes = await _restauranteConsumer.GetAllAsync();
        var activos = restaurantes.Where(r => r.Abierto || r.Estado == Delivery.Modelos.Enums.EstadoRestauranteEnum.Aprobado);
        return View(activos);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
