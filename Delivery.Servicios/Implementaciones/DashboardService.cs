using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Enums;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class DashboardService : IDashboardService
    {
        private readonly DeliveryDbContext _context;

        public DashboardService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardEstadisticasDto> ObtenerEstadisticasAsync()
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var estadisticas = new DashboardEstadisticasDto
            {
                UsuariosRegistrados = await _context.Set<Delivery.Modelos.Entidades.Usuario>().CountAsync(),
                RestaurantesActivos = await _context.Set<Delivery.Modelos.Entidades.Restaurante>().CountAsync(r => r.Estado == EstadoRestauranteEnum.Aprobado),
                ProductosRegistrados = await _context.Set<Delivery.Modelos.Entidades.Producto>().CountAsync(),
                ProductosDisponibles = await _context.Set<Delivery.Modelos.Entidades.Producto>().CountAsync(p => p.Disponible),
                PedidosDelDia = await _context.Set<Delivery.Modelos.Entidades.Pedido>().CountAsync(p => p.FechaPedido >= hoy),
                PedidosPendientes = await _context.Set<Delivery.Modelos.Entidades.Pedido>().CountAsync(p => p.EstadoPedido == EstadoPedidoEnum.Pendiente || p.EstadoPedido == EstadoPedidoEnum.EnPreparacion),
                PedidosEntregados = await _context.Set<Delivery.Modelos.Entidades.Pedido>().CountAsync(p => p.EstadoPedido == EstadoPedidoEnum.Entregado),
                RepartidoresActivos = await _context.Set<Delivery.Modelos.Entidades.Repartidor>().CountAsync(r => r.Estado == Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible),
                
                VentasDelDia = await _context.Set<Delivery.Modelos.Entidades.Pedido>()
                                    .Where(p => p.EstadoPedido == EstadoPedidoEnum.Entregado && p.FechaPedido >= hoy)
                                    .SumAsync(p => p.Total),
                                    
                VentasDelMes = await _context.Set<Delivery.Modelos.Entidades.Pedido>()
                                    .Where(p => p.EstadoPedido == EstadoPedidoEnum.Entregado && p.FechaPedido >= inicioMes)
                                    .SumAsync(p => p.Total)
            };

            // Restaurantes con más pedidos (top 5)
            estadisticas.RestaurantesConMasPedidos = await _context.Set<Delivery.Modelos.Entidades.Pedido>()
                .GroupBy(p => p.Restaurante!.Nombre)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToListAsync();

            // Productos más vendidos (top 5)
            estadisticas.ProductosMasVendidos = await _context.Set<Delivery.Modelos.Entidades.DetallePedido>()
                .GroupBy(d => d.Producto!.Nombre)
                .OrderByDescending(g => g.Sum(d => d.Cantidad))
                .Take(5)
                .Select(g => g.Key)
                .ToListAsync();

            // Clientes con más compras (top 5)
            estadisticas.ClientesConMasCompras = await _context.Set<Delivery.Modelos.Entidades.Pedido>()
                .GroupBy(p => p.Usuario!.Nombre)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToListAsync();

            return estadisticas;
        }
    }
}
