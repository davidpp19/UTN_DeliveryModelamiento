using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        Task<Pedido> ActualizarEstadoRestauranteAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long restauranteId);
        Task<Pedido> ActualizarEstadoRepartidorAsync(long pedidoId, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long repartidorId);
        Task<IEnumerable<Pedido>> GetHistorialUsuarioAsync(long usuarioId);
    }
}
