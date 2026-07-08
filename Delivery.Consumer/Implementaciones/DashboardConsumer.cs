using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class DashboardConsumer : IDashboardConsumer
    {
        private readonly HttpClient _httpClient;

        public DashboardConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardEstadisticasDto?> ObtenerEstadisticasAsync()
        {
            return await _httpClient.GetFromJsonAsync<DashboardEstadisticasDto>("api/Dashboard");
        }
    }
}
