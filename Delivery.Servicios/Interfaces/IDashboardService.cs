using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardEstadisticasDto> ObtenerEstadisticasAsync();
    }
}
