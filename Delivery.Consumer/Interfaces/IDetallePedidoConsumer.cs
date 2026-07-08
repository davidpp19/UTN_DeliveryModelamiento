using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IDetallePedidoConsumer
    {
        Task<IEnumerable<DetallePedido>> GetAllAsync();
        Task<DetallePedido?> GetByIdAsync(long id);
        Task<DetallePedido> CreateAsync(DetallePedido entity);
        Task<bool> UpdateAsync(long id, DetallePedido entity);
        Task<bool> DeleteAsync(long id);
    }
}
