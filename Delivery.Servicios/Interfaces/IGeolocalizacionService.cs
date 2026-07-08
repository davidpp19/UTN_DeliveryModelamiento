using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IGeolocalizacionService
    {
        Task ActualizarUbicacionRepartidorAsync(ActualizacionUbicacionDto actualizacion);
        Task<CoordenadasDto?> ObtenerUbicacionRepartidorAsync(long repartidorId);
    }
}
