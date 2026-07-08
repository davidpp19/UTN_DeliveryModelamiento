using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class DireccionConsumer : IDireccionConsumer
    {
        private readonly HttpClient _httpClient;

        public DireccionConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Direccion>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Direccion>>("api/Direcciones") ?? new List<Direccion>();
        }

        public async Task<Direccion?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Direccion>($"api/Direcciones/{id}");
        }

        public async Task<Direccion> CreateAsync(Direccion entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Direcciones", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Direccion>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Direccion entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Direcciones/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Direcciones/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
