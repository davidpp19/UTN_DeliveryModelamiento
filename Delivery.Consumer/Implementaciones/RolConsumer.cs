using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class RolConsumer : IRolConsumer
    {
        private readonly HttpClient _httpClient;

        public RolConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Rol>>("api/Roles") ?? new List<Rol>();
        }

        public async Task<Rol?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Rol>($"api/Roles/{id}");
        }

        public async Task<Rol> CreateAsync(Rol entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Roles", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Rol>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Rol entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Roles/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Roles/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
