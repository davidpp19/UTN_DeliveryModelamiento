using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusquedasController : ControllerBase
    {
        private readonly IBusquedaService _busquedaService;

        public BusquedasController(IBusquedaService busquedaService)
        {
            _busquedaService = busquedaService;
        }

        [HttpGet("restaurantes")]
        public async Task<ActionResult<IEnumerable<Restaurante>>> BuscarRestaurantes([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarRestaurantesAsync(filtros);
            return Ok(resultados);
        }

        [HttpGet("productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarProductosAsync(filtros);
            return Ok(resultados);
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<CategoriaProducto>>> BuscarCategorias([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarCategoriasAsync(filtros);
            return Ok(resultados);
        }

        [HttpGet("pedidos")]
        public async Task<ActionResult<IEnumerable<Pedido>>> BuscarPedidos([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarPedidosAsync(filtros);
            return Ok(resultados);
        }

        [HttpGet("clientes")]
        public async Task<ActionResult<IEnumerable<Usuario>>> BuscarClientes([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarClientesAsync(filtros);
            return Ok(resultados);
        }

        [HttpGet("repartidores")]
        public async Task<ActionResult<IEnumerable<Repartidor>>> BuscarRepartidores([FromQuery] FiltrosBusquedaDto filtros)
        {
            var resultados = await _busquedaService.BuscarRepartidoresAsync(filtros);
            return Ok(resultados);
        }
    }
}
