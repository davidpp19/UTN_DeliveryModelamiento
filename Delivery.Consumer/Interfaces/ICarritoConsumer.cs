using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICarritoConsumer
    {
        Task<Carrito?> GetCarritoAsync(long usuarioId);
        Task<Carrito?> AgregarProductoAsync(AgregarAlCarritoDto dto);
        Task<bool> QuitarProductoAsync(long usuarioId, long carritoItemId);
        Task<bool> VaciarCarritoAsync(long usuarioId);
    }
}
