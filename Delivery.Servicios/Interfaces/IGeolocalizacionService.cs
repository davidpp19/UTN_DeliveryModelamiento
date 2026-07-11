using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IGeolocalizacionService
    {
        Task ActualizarUbicacionRepartidorAsync(ActualizacionUbicacionDto actualizacion);
        Task<CoordenadasDto?> ObtenerUbicacionRepartidorAsync(long repartidorId);
        double CalcularDistanciaKm(double lat1, double lon1, double lat2, double lon2);
        decimal CalcularCostoEnvio(double distanciaKm);
    }
}
