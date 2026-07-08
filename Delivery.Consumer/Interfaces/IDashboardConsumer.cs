using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Consumer.Interfaces
{
    public interface IDashboardConsumer
    {
        Task<DashboardEstadisticasDto?> ObtenerEstadisticasAsync();
    }
}
