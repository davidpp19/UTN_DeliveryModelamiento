using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IDetallePedidoService
    {
        Task<IEnumerable<DetallePedido>> GetAllAsync();
        Task<DetallePedido?> GetByIdAsync(long id);
        Task<DetallePedido> CreateAsync(DetallePedido detalle);
        Task<DetallePedido> UpdateAsync(DetallePedido detalle);
        Task<bool> DeleteAsync(long id);
    }
}
