using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class PagoConsumer : IPagoConsumer
    {
        private readonly HttpClient _httpClient;

        public PagoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Pago>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Pago>>("api/Pagos") ?? new List<Pago>();
        }

        public async Task<Pago?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Pago>($"api/Pagos/{id}");
        }

        public async Task<Pago> CreateAsync(Pago entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Pagos", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Pago>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Pago entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Pagos/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Pagos/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
