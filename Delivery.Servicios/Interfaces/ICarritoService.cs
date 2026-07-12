using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface ICarritoService
    {
        Task<Carrito> AgregarProductoAsync(AgregarAlCarritoDto dto);
        Task<Carrito?> ObtenerCarritoActivoAsync(long usuarioId);
        Task<bool> QuitarProductoAsync(long usuarioId, long carritoItemId);
        Task<bool> VaciarCarritoAsync(long usuarioId);
    }
}
