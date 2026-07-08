using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Enums;
using Delivery.Modelos.Excepciones;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class CarritoService : ICarritoService
    {
        private readonly DeliveryDbContext _context;

        public CarritoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido> AgregarProductoAsync(AgregarAlCarritoDto dto)
        {
            var restaurante = await _context.Restaurantes.FindAsync(dto.RestauranteId);
            if (restaurante == null || !restaurante.Abierto)
                throw new BusinessException("El restaurante no está disponible.");

            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null || !producto.Disponible)
                throw new BusinessException("El producto no está disponible.");

            if (producto.RestauranteId != dto.RestauranteId)
                throw new BusinessException("El producto no pertenece al restaurante especificado.");

            var carrito = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.UsuarioId == dto.UsuarioId && p.EstadoPedido == EstadoPedidoEnum.Borrador);

            if (carrito == null)
            {
                carrito = new Pedido
                {
                    UsuarioId = dto.UsuarioId,
                    RestauranteId = dto.RestauranteId,
                    EstadoPedido = EstadoPedidoEnum.Borrador,
                    FechaPedido = DateTime.UtcNow,
                    Total = 0
                };
                _context.Pedidos.Add(carrito);
                await _context.SaveChangesAsync();
            }
            else if (carrito.RestauranteId != dto.RestauranteId)
            {
                throw new BusinessException("No puedes mezclar productos de diferentes restaurantes en un mismo pedido.");
            }

            var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == dto.ProductoId);
            if (detalle != null)
            {
                detalle.Cantidad += dto.Cantidad;
            }
            else
            {
                detalle = new DetallePedido
                {
                    PedidoId = carrito.Id,
                    ProductoId = dto.ProductoId,
                    Cantidad = dto.Cantidad,
                    PrecioUnitario = producto.Precio
                };
                _context.DetallesPedido.Add(detalle);
            }

            await _context.SaveChangesAsync();

            // Recalcular Total
            carrito.Total = carrito.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            _context.Pedidos.Update(carrito);
            await _context.SaveChangesAsync();

            return carrito;
        }

        public async Task<Pedido?> ObtenerCarritoActivoAsync(long usuarioId)
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.EstadoPedido == EstadoPedidoEnum.Borrador);
        }

        public async Task<bool> QuitarProductoAsync(long usuarioId, long detallePedidoId)
        {
            var carrito = await ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null) return false;

            var detalle = carrito.Detalles.FirstOrDefault(d => d.Id == detallePedidoId);
            if (detalle == null) return false;

            _context.DetallesPedido.Remove(detalle);
            await _context.SaveChangesAsync();

            carrito.Total = carrito.Detalles.Where(d => d.Id != detallePedidoId).Sum(d => d.Cantidad * d.PrecioUnitario);
            
            if (!carrito.Detalles.Any(d => d.Id != detallePedidoId))
            {
                _context.Pedidos.Remove(carrito);
            }
            else
            {
                _context.Pedidos.Update(carrito);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Pedido> ConfirmarCarritoAsync(long usuarioId, long direccionId)
        {
            var carrito = await ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null || !carrito.Detalles.Any())
                throw new BusinessException("El carrito está vacío o no existe.");

            carrito.DireccionEntregaId = direccionId;
            carrito.EstadoPedido = EstadoPedidoEnum.Pendiente; // Pasa a estado de restaurante
            
            _context.Pedidos.Update(carrito);
            await _context.SaveChangesAsync();

            return carrito;
        }
    }
}
