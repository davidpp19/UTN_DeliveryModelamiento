using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Consumer.Interfaces;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Implementaciones
{
    public class AdminAprobacionesConsumer : IAdminAprobacionesConsumer
    {
        private readonly HttpClient _httpClient;

        public AdminAprobacionesConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Repartidor>> GetRepartidoresPendientesAsync()
        {
            var response = await _httpClient.GetAsync("api/AdminAprobaciones/repartidores/pendientes");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Repartidor>>() ?? new List<Repartidor>();
            }
            return new List<Repartidor>();
        }

        public async Task<IEnumerable<Restaurante>> GetRestaurantesPendientesAsync()
        {
            var response = await _httpClient.GetAsync("api/AdminAprobaciones/restaurantes/pendientes");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Restaurante>>() ?? new List<Restaurante>();
            }
            return new List<Restaurante>();
        }

        public async Task<bool> AprobarRepartidorAsync(long id)
        {
            var response = await _httpClient.PostAsync($"api/AdminAprobaciones/repartidores/{id}/aprobar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RechazarRepartidorAsync(long id)
        {
            var response = await _httpClient.PostAsync($"api/AdminAprobaciones/repartidores/{id}/rechazar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AprobarRestauranteAsync(long id)
        {
            var response = await _httpClient.PostAsync($"api/AdminAprobaciones/restaurantes/{id}/aprobar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RechazarRestauranteAsync(long id)
        {
            var response = await _httpClient.PostAsync($"api/AdminAprobaciones/restaurantes/{id}/rechazar", null);
            return response.IsSuccessStatusCode;
        }
    }
}
