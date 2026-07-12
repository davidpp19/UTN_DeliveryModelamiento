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

        public async Task<Carrito> AgregarProductoAsync(AgregarAlCarritoDto dto)
        {
            var restaurante = await _context.Restaurantes.FindAsync(dto.RestauranteId);
            if (restaurante == null || !restaurante.Abierto)
                throw new BusinessException("El restaurante no está disponible.");

            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null || !producto.Disponible)
                throw new BusinessException("El producto no está disponible.");

            if (producto.RestauranteId != dto.RestauranteId)
                throw new BusinessException("El producto no pertenece al restaurante especificado.");

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == dto.UsuarioId);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = dto.UsuarioId,
                    RestauranteId = dto.RestauranteId,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }
            else if (carrito.RestauranteId != dto.RestauranteId)
            {
                throw new BusinessException("No puedes mezclar productos de diferentes restaurantes en un mismo pedido.");
            }

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
            if (item != null)
            {
                item.Cantidad += dto.Cantidad;
            }
            else
            {
                item = new CarritoItem
                {
                    CarritoId = carrito.Id,
                    ProductoId = dto.ProductoId,
                    Cantidad = dto.Cantidad,
                    PrecioUnitario = producto.Precio
                };
                _context.CarritoItems.Add(item);
            }

            carrito.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return carrito;
        }

        public async Task<Carrito?> ObtenerCarritoActivoAsync(long usuarioId)
        {
            return await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }

        public async Task<bool> QuitarProductoAsync(long usuarioId, long carritoItemId)
        {
            var carrito = await ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null) return false;

            var item = carrito.Items.FirstOrDefault(i => i.Id == carritoItemId);
            if (item == null) return false;

            _context.CarritoItems.Remove(item);

            if (carrito.Items.Count(i => i.Id != carritoItemId) == 0)
            {
                _context.Carritos.Remove(carrito);
            }
            else
            {
                carrito.FechaActualizacion = DateTime.UtcNow;
                // No need to call Update, EF Core tracks the change
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VaciarCarritoAsync(long usuarioId)
        {
            var carrito = await ObtenerCarritoActivoAsync(usuarioId);
            if (carrito == null) return false;

            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
