using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class RestauranteConsumer : IRestauranteConsumer
    {
        private readonly HttpClient _httpClient;

        public RestauranteConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Restaurante>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Restaurante>>("api/Restaurantes") ?? new List<Restaurante>();
        }

        public async Task<Restaurante?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Restaurante>($"api/Restaurantes/{id}");
        }

        public async Task<Restaurante> CreateAsync(Restaurante entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Restaurantes", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Restaurante>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Restaurante entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Restaurantes/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Restaurantes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
