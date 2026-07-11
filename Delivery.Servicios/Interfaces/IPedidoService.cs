using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(long id);
        Task<Pedido> CreateAsync(Pedido pedido);
        Task<Pedido> UpdateAsync(Pedido pedido);
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Crea el Pedido real desde los items del carrito de sesión.
        /// Este es el ÚNICO punto donde se inserta en la tabla pedidos.
        /// </summary>
        Task<Pedido> CrearDesdeSesionAsync(long usuarioId, long direccionId, Delivery.Modelos.Enums.TipoMetodoPagoEnum metodoPago, CarritoSesionDto carrito);

        Task<Pedido> ActualizarEstadoRestauranteAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long restauranteId);
        Task<Pedido> ActualizarEstadoRepartidorAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long repartidorId);
        Task<Pedido> AsignarPedidoAsync(long pedidoId, long repartidorId);
        Task<IEnumerable<Pedido>> GetHistorialUsuarioAsync(long usuarioId);
    }
}

