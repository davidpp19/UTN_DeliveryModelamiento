using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class ResenaConsumer : IResenaConsumer
    {
        private readonly HttpClient _httpClient;

        public ResenaConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Resena>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Resena>>("api/Resenas") ?? new List<Resena>();
        }

        public async Task<Resena?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Resena>($"api/Resenas/{id}");
        }

        public async Task<Resena> CreateAsync(Resena entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Resenas", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Resena>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Resena entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Resenas/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Resenas/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
