using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;
using Delivery.Modelos.Enums;

namespace Delivery.Servicios.Implementaciones
{
    public class BusquedaService : IBusquedaService
    {
        private readonly DeliveryDbContext _context;

        public BusquedaService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurante>> BuscarRestaurantesAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Restaurantes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.TerminoBusqueda))
            {
                var termino = filtros.TerminoBusqueda.ToLower();
                query = query.Where(r => r.Nombre.ToLower().Contains(termino) || 
                                         (r.Descripcion != null && r.Descripcion.ToLower().Contains(termino)));
            }

            if (!string.IsNullOrWhiteSpace(filtros.Categoria))
            {
                query = query.Where(r => r.Categoria == filtros.Categoria);
            }

            if (filtros.Disponibilidad.HasValue)
            {
                query = query.Where(r => r.Abierto == filtros.Disponibilidad.Value);
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> BuscarProductosAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Productos.Include(p => p.Restaurante).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.TerminoBusqueda))
            {
                var termino = filtros.TerminoBusqueda.ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(termino) || 
                                         (p.Descripcion != null && p.Descripcion.ToLower().Contains(termino)));
            }

            if (filtros.RestauranteId.HasValue)
            {
                query = query.Where(p => p.RestauranteId == filtros.RestauranteId.Value);
            }

            if (filtros.PrecioMinimo.HasValue)
            {
                query = query.Where(p => p.Precio >= filtros.PrecioMinimo.Value);
            }

            if (filtros.PrecioMaximo.HasValue)
            {
                query = query.Where(p => p.Precio <= filtros.PrecioMaximo.Value);
            }

            if (filtros.Disponibilidad.HasValue)
            {
                query = query.Where(p => p.Disponible == filtros.Disponibilidad.Value);
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }

        public async Task<IEnumerable<CategoriaProducto>> BuscarCategoriasAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Set<CategoriaProducto>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.TerminoBusqueda))
            {
                var termino = filtros.TerminoBusqueda.ToLower();
                query = query.Where(c => c.Nombre.ToLower().Contains(termino));
            }

            if (filtros.RestauranteId.HasValue)
            {
                query = query.Where(c => c.RestauranteId == filtros.RestauranteId.Value);
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> BuscarPedidosAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Pedidos
                                .Include(p => p.Usuario)
                                .Include(p => p.Restaurante)
                                .AsQueryable();

            if (filtros.RestauranteId.HasValue)
            {
                query = query.Where(p => p.RestauranteId == filtros.RestauranteId.Value);
            }

            if (filtros.EstadoPedido.HasValue)
            {
                query = query.Where(p => p.EstadoPedido == filtros.EstadoPedido.Value);
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> BuscarClientesAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Usuarios.Include(u => u.Rol).AsQueryable();

            // Asumiendo que buscamos solo Clientes (filtrado por rol en memoria o por nombre)
            if (!string.IsNullOrWhiteSpace(filtros.TerminoBusqueda))
            {
                var termino = filtros.TerminoBusqueda.ToLower();
                query = query.Where(u => u.Nombre.ToLower().Contains(termino) || 
                                         (u.Email != null && u.Email.ToLower().Contains(termino)));
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Repartidor>> BuscarRepartidoresAsync(FiltrosBusquedaDto filtros)
        {
            var query = _context.Set<Repartidor>().Include(r => r.Usuario).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.TerminoBusqueda))
            {
                var termino = filtros.TerminoBusqueda.ToLower();
                query = query.Where(r => r.Usuario != null && 
                                         (r.Usuario.Nombre.ToLower().Contains(termino) || 
                                          (r.Usuario.Email != null && r.Usuario.Email.ToLower().Contains(termino))));
            }

            if (filtros.Disponibilidad.HasValue)
            {
                query = query.Where(r => r.Estado == (filtros.Disponibilidad.Value ? Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible : Delivery.Modelos.Enums.EstadoRepartidorEnum.Desconectado));
            }

            return await query.Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
                              .Take(filtros.TamanoPagina)
                              .ToListAsync();
        }
    }
}
