using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class ProductoConsumer : IProductoConsumer
    {
        private readonly HttpClient _httpClient;

        public ProductoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Producto>>("api/Productos") ?? new List<Producto>();
        }

        public async Task<Producto?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Producto>($"api/Productos/{id}");
        }

        public async Task<Producto> CreateAsync(Producto entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Productos", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Producto>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Producto entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Productos/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Productos/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
