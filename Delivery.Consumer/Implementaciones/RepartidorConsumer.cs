using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class RepartidorConsumer : IRepartidorConsumer
    {
        private readonly HttpClient _httpClient;

        public RepartidorConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Repartidor>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Repartidor>>("api/Repartidores") ?? new List<Repartidor>();
        }

        public async Task<Repartidor?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Repartidor>($"api/Repartidores/{id}");
        }

        public async Task<Repartidor> CreateAsync(Repartidor entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Repartidores", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Repartidor>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Repartidor entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Repartidores/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Repartidores/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
