using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class CuponConsumer : ICuponConsumer
    {
        private readonly HttpClient _httpClient;

        public CuponConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Cupon>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Cupon>>("api/Cupones") ?? new List<Cupon>();
        }

        public async Task<Cupon?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Cupon>($"api/Cupones/{id}");
        }

        public async Task<Cupon> CreateAsync(Cupon entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Cupones", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Cupon>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Cupon entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Cupones/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Cupones/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
