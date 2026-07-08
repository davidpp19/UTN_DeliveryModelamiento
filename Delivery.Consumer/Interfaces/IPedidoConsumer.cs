using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IPedidoConsumer
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(long id);
        Task<Pedido> CreateAsync(Pedido entity);
        Task<bool> UpdateAsync(long id, Pedido entity);
        Task<bool> DeleteAsync(long id);
        Task<Pedido?> ActualizarEstadoRestauranteAsync(long id, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long restauranteId);
        Task<Pedido?> ActualizarEstadoRepartidorAsync(long id, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long repartidorId);
    }
}
