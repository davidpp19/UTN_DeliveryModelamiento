using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class HistorialAsignacionesRepartidorConsumer : IHistorialAsignacionesRepartidorConsumer
    {
        private readonly HttpClient _httpClient;

        public HistorialAsignacionesRepartidorConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<HistorialAsignacionesRepartidor>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<HistorialAsignacionesRepartidor>>("api/HistorialAsignaciones") ?? new List<HistorialAsignacionesRepartidor>();
        }

        public async Task<HistorialAsignacionesRepartidor?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<HistorialAsignacionesRepartidor>($"api/HistorialAsignaciones/{id}");
        }

        public async Task<HistorialAsignacionesRepartidor> CreateAsync(HistorialAsignacionesRepartidor entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HistorialAsignaciones", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HistorialAsignacionesRepartidor>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, HistorialAsignacionesRepartidor entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HistorialAsignaciones/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/HistorialAsignaciones/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
