using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Modelos.Excepciones;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class PedidoService : IPedidoService
    {
        private readonly DeliveryDbContext _context;

        public PedidoService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Pagos)
                .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(long id)
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Pagos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pedido> CreateAsync(Pedido pedido)
        {
            pedido.FechaPedido = System.DateTime.UtcNow;
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<Pedido> UpdateAsync(Pedido pedido)
        {
            var existing = await _context.Pedidos.FindAsync(pedido.Id);
            if (existing == null) return null!;

            existing.UsuarioId = pedido.UsuarioId;
            existing.RestauranteId = pedido.RestauranteId;
            existing.RepartidorId = pedido.RepartidorId;
            existing.DireccionEntregaId = pedido.DireccionEntregaId;
            existing.EstadoPedido = pedido.EstadoPedido;
            existing.Subtotal = pedido.Subtotal;
            existing.CostoEnvio = pedido.CostoEnvio;
            existing.Total = pedido.Total;
            existing.TipoMetodoPago = pedido.TipoMetodoPago;
            existing.MetodoPagoId = pedido.MetodoPagoId;
            existing.CuponId = pedido.CuponId;
            existing.MontoDescuento = pedido.MontoDescuento;
            existing.Notas = pedido.Notas;
            existing.FechaEntregaEstimada = pedido.FechaEntregaEstimada;
            existing.FechaEntregaReal = pedido.FechaEntregaReal;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Crea el Pedido real desde el carrito de sesión (en memoria).
        /// Se ejecuta DENTRO de una transacción atómica.
        /// NO hay ninguna inserción en pedidos antes de este método.
        /// </summary>
        public async Task<Pedido> CrearDesdeSesionAsync(long usuarioId, long direccionId, CarritoSesionDto carritoSesion)
        {
            if (carritoSesion == null || !carritoSesion.Items.Any())
                throw new BusinessException("El carrito está vacío.");

            // Verificar que la dirección exista y pertenezca al usuario
            var direccion = await _context.Direcciones.FindAsync(direccionId);
            if (direccion == null)
                throw new BusinessException("La dirección de entrega no existe.");
            if (direccion.UsuarioId != usuarioId)
                throw new BusinessException("La dirección de entrega no pertenece al usuario.");

            // Verificar que el restaurante exista y esté abierto
            var restaurante = await _context.Restaurantes.FindAsync(carritoSesion.RestauranteId);
            if (restaurante == null)
                throw new BusinessException("El restaurante no existe.");

            var ahora = DateTime.UtcNow;
            var subtotal = carritoSesion.Items.Sum(i => i.PrecioUnitario * i.Cantidad);
            var costoEnvio = restaurante.CostoEnvioBase;
            var total = subtotal + costoEnvio;

            // Usar transacción para garantizar atomicidad
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear el Pedido — aquí es la ÚNICA vez que se inserta en la tabla pedidos
                var pedido = new Pedido
                {
                    UsuarioId          = usuarioId,
                    RestauranteId      = carritoSesion.RestauranteId,
                    DireccionEntregaId = direccionId,        // FK válida y verificada
                    EstadoPedido       = EstadoPedidoEnum.Pendiente,
                    Subtotal           = subtotal,
                    CostoEnvio         = costoEnvio,
                    Total              = total,
                    TipoMetodoPago     = TipoMetodoPagoEnum.Efectivo,
                    FechaPedido        = ahora
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync(); // genera el Id del pedido

                // Crear los DetallePedido
                foreach (var item in carritoSesion.Items)
                {
                    var detalle = new DetallePedido
                    {
                        PedidoId       = pedido.Id,
                        ProductoId     = item.ProductoId,
                        Cantidad       = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    };
                    _context.DetallesPedido.Add(detalle);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Devolver el pedido con sus detalles cargados
                return await _context.Pedidos
                    .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                    .Include(p => p.Restaurante)
                    .Include(p => p.DireccionEntrega)
                    .FirstAsync(p => p.Id == pedido.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Pedido> ActualizarEstadoRestauranteAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long restauranteId)

        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null) throw new Delivery.Modelos.Excepciones.BusinessException("Pedido no encontrado.");
            
            if (pedido.RestauranteId != restauranteId)
                throw new Delivery.Modelos.Excepciones.BusinessException("El pedido no pertenece a este restaurante.");

            if (nuevoEstado != Delivery.Modelos.Enums.EstadoPedidoEnum.Aceptado &&
                nuevoEstado != Delivery.Modelos.Enums.EstadoPedidoEnum.EnPreparacion &&
                nuevoEstado != Delivery.Modelos.Enums.EstadoPedidoEnum.Cancelado)
            {
                throw new Delivery.Modelos.Excepciones.BusinessException("Estado no válido para el restaurante.");
            }

            pedido.EstadoPedido = nuevoEstado;
            pedido.ActualizadoEn = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return pedido;
        }

        public async Task<Pedido> ActualizarEstadoRepartidorAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long repartidorId)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null) throw new Delivery.Modelos.Excepciones.BusinessException("Pedido no encontrado.");
            
            if (pedido.RepartidorId != repartidorId)
                throw new Delivery.Modelos.Excepciones.BusinessException("El pedido no está asignado a este repartidor.");

            if (nuevoEstado != Delivery.Modelos.Enums.EstadoPedidoEnum.EnCamino &&
                nuevoEstado != Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado)
            {
                throw new Delivery.Modelos.Excepciones.BusinessException("Estado no válido para el repartidor.");
            }

            pedido.EstadoPedido = nuevoEstado;
            if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado)
            {
                pedido.FechaEntregaReal = System.DateTime.UtcNow;
            }

            pedido.ActualizadoEn = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return pedido;
        }

        public async Task<IEnumerable<Pedido>> GetHistorialUsuarioAsync(long usuarioId)
        {
            return await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Include(p => p.Restaurante)
                .Where(p => p.UsuarioId == usuarioId && 
                           (p.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado || 
                            p.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Cancelado))
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }
    }
}
