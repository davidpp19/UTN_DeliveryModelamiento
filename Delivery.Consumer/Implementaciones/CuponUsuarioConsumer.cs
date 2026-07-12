using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class CuponUsuarioConsumer : ICuponUsuarioConsumer
    {
        private readonly HttpClient _httpClient;

        public CuponUsuarioConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CuponUsuario>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CuponUsuario>>("api/CuponesUsuarios") ?? new List<CuponUsuario>();
        }

        public async Task<CuponUsuario?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<CuponUsuario>($"api/CuponesUsuarios/{id}");
        }

        public async Task<CuponUsuario> CreateAsync(CuponUsuario entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CuponesUsuarios", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CuponUsuario>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, CuponUsuario entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/CuponesUsuarios/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/CuponesUsuarios/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
