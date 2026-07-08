using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IBusquedaService
    {
        Task<IEnumerable<Restaurante>> BuscarRestaurantesAsync(FiltrosBusquedaDto filtros);
        Task<IEnumerable<Producto>> BuscarProductosAsync(FiltrosBusquedaDto filtros);
        Task<IEnumerable<CategoriaProducto>> BuscarCategoriasAsync(FiltrosBusquedaDto filtros);
        Task<IEnumerable<Pedido>> BuscarPedidosAsync(FiltrosBusquedaDto filtros);
        Task<IEnumerable<Usuario>> BuscarClientesAsync(FiltrosBusquedaDto filtros);
        Task<IEnumerable<Repartidor>> BuscarRepartidoresAsync(FiltrosBusquedaDto filtros);
    }
}
