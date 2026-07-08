using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICarritoConsumer
    {
        Task<Pedido?> GetCarritoAsync(long usuarioId);
        Task<Pedido?> AgregarProductoAsync(AgregarAlCarritoDto dto);
        Task<bool> QuitarProductoAsync(long usuarioId, long detallePedidoId);
        Task<Pedido?> ConfirmarCarritoAsync(long usuarioId, long direccionId);
    }
}
