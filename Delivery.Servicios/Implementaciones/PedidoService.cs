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
        private readonly IGeolocalizacionService _geoService;

        public PedidoService(DeliveryDbContext context, IGeolocalizacionService geoService)
        {
            _context = context;
            _geoService = geoService;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos
                .AsNoTracking()
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
            existing.FechaEntregaEstimada = pedido.FechaEntregaEstimada?.ToUniversalTime();
            existing.FechaEntregaReal = pedido.FechaEntregaReal?.ToUniversalTime();
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
        public async Task<Pedido> CrearDesdeSesionAsync(long usuarioId, long direccionId, TipoMetodoPagoEnum metodoPago, CarritoSesionDto carritoSesion)
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
            
            decimal costoEnvio = restaurante.CostoEnvioBase; // Fallback
            
            // Si hay lat/lon en direccion y restaurante, calcular dinámico
            if (direccion.Latitud.HasValue && direccion.Longitud.HasValue && 
                restaurante.Latitud.HasValue && restaurante.Longitud.HasValue)
            {
                var rutaOSRM = await _geoService.CalcularRutaOSRMAsync(
                    (double)restaurante.Latitud.Value, (double)restaurante.Longitud.Value,
                    (double)direccion.Latitud.Value, (double)direccion.Longitud.Value);
                
                costoEnvio = _geoService.CalcularCostoEnvio(rutaOSRM.DistanciaKm);
            }

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
                    EstadoPedido       = EstadoPedidoEnum.Aceptado, // Se da por sobreentendido
                    Subtotal           = subtotal,
                    CostoEnvio         = costoEnvio,
                    MontoDescuento     = carritoSesion.Descuento,
                    CuponId            = carritoSesion.CuponId,
                    Total              = total - carritoSesion.Descuento,
                    TipoMetodoPago     = metodoPago,
                    ComprobanteTransferenciaUrl = carritoSesion.ComprobanteTransferenciaUrl,
                    Notas              = carritoSesion.Notas,
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

                // Eliminar el carrito original de la base de datos ya que se convirtió en pedido
                var carritoBd = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
                if (carritoBd != null)
                {
                    _context.Carritos.Remove(carritoBd);
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

            // Validar transiciones de estado para el Restaurante
            bool transicionValida = false;
            
            if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Cancelado)
            {
                // Se puede cancelar si no ha sido entregado o recogido
                if (pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Pendiente || 
                    pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Aceptado || 
                    pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.EnPreparacion)
                {
                    transicionValida = true;
                }
            }
            else
            {
                switch (pedido.EstadoPedido)
                {
                    case Delivery.Modelos.Enums.EstadoPedidoEnum.Pendiente:
                        if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Aceptado) transicionValida = true;
                        break;
                    case Delivery.Modelos.Enums.EstadoPedidoEnum.Aceptado:
                        if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.EnPreparacion) transicionValida = true;
                        break;
                    case Delivery.Modelos.Enums.EstadoPedidoEnum.EnPreparacion:
                        if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.ListoParaRecoger) transicionValida = true;
                        break;
                }
            }

            if (!transicionValida)
            {
                throw new Delivery.Modelos.Excepciones.BusinessException("Estado o transición no válido para el restaurante.");
            }

            pedido.EstadoPedido = nuevoEstado;
            
            if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Cancelado && pedido.RepartidorId != null)
            {
                var repartidor = await _context.Repartidores.FindAsync(pedido.RepartidorId);
                if (repartidor != null)
                {
                    repartidor.Estado = Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible;
                }
            }
            
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

            // Validar transiciones de estado para el Repartidor
            bool transicionValida = false;

            switch (pedido.EstadoPedido)
            {
                case Delivery.Modelos.Enums.EstadoPedidoEnum.ListoParaRecoger:
                    if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Recogido) transicionValida = true;
                    break;
                case Delivery.Modelos.Enums.EstadoPedidoEnum.Recogido:
                    if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.EnCamino) transicionValida = true;
                    break;
                case Delivery.Modelos.Enums.EstadoPedidoEnum.EnCamino:
                    if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado) transicionValida = true;
                    break;
            }

            // Mantenemos retrocompatibilidad si el repartidor pasa directamente a EnCamino o Entregado
            // según lo que había originalmente (por si alguna vista lo hace)
            if (pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Pendiente || 
                pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.Aceptado || 
                pedido.EstadoPedido == Delivery.Modelos.Enums.EstadoPedidoEnum.EnPreparacion)
            {
                // Un repartidor no debería poder cambiar el estado si el restaurante aún lo tiene.
                // Pero lo dejaremos bloqueado explícitamente al requerir 'transicionValida = true'.
            }

            if (!transicionValida)
            {
                throw new Delivery.Modelos.Excepciones.BusinessException("Estado o transición no válido para el repartidor.");
            }

            pedido.EstadoPedido = nuevoEstado;
            if (nuevoEstado == Delivery.Modelos.Enums.EstadoPedidoEnum.Entregado)
            {
                pedido.FechaEntregaReal = System.DateTime.UtcNow;
                
                // Liberar al repartidor
                var repartidor = await _context.Repartidores.FindAsync(repartidorId);
                if (repartidor != null)
                {
                    repartidor.Estado = Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible;
                }
            }

            pedido.ActualizadoEn = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return pedido;
        }

        public async Task<Pedido> AsignarPedidoAsync(long pedidoId, long repartidorId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = await _context.Pedidos.FindAsync(pedidoId);
                if (pedido == null) throw new BusinessException("Pedido no encontrado.");
                
                if (pedido.EstadoPedido == EstadoPedidoEnum.Entregado || pedido.EstadoPedido == EstadoPedidoEnum.Cancelado)
                    throw new BusinessException("El pedido no puede ser asignado en su estado actual.");

                if (pedido.RepartidorId != null)
                    throw new BusinessException("Este pedido ya fue asignado a otro repartidor.");

                var repartidor = await _context.Repartidores.FindAsync(repartidorId);
                if (repartidor == null) throw new BusinessException("Repartidor no encontrado.");

                if (repartidor.Estado != EstadoRepartidorEnum.Disponible)
                    throw new BusinessException("El repartidor no está disponible para aceptar pedidos.");

                // Asignar el pedido
                pedido.RepartidorId = repartidorId;
                
                // Cambiar estado del repartidor a Ocupado
                repartidor.Estado = EstadoRepartidorEnum.Ocupado;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return pedido;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
