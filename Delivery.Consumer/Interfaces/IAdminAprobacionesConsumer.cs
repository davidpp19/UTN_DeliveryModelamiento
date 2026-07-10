using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IAdminAprobacionesConsumer
    {
        Task<IEnumerable<Delivery.Modelos.DTOs.RepartidorPendienteDto>> GetRepartidoresPendientesAsync();
        Task<IEnumerable<Delivery.Modelos.DTOs.RestaurantePendienteDto>> GetRestaurantesPendientesAsync();
        Task<bool> AprobarRepartidorAsync(long id);
        Task<bool> RechazarRepartidorAsync(long id, string? motivo = null);
        Task<bool> AprobarRestauranteAsync(long id);
        Task<bool> RechazarRestauranteAsync(long id, string? motivo = null);
    }
}
