using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IHomeService
    {
        Task<HomeResponseDto> ObtenerDatosHomeAsync();
    }
}
