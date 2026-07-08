using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICarritoService
    {
        Task<Pedido> AgregarProductoAsync(AgregarAlCarritoDto dto);
        Task<Pedido?> ObtenerCarritoActivoAsync(long usuarioId);
        Task<bool> QuitarProductoAsync(long usuarioId, long detallePedidoId);
        Task<Pedido> ConfirmarCarritoAsync(long usuarioId, long direccionId);
    }
}
