using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class CategoriaProductoConsumer : ICategoriaProductoConsumer
    {
        private readonly HttpClient _httpClient;

        public CategoriaProductoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CategoriaProducto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CategoriaProducto>>("api/CategoriasProducto") ?? new List<CategoriaProducto>();
        }

        public async Task<CategoriaProducto?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<CategoriaProducto>($"api/CategoriasProducto/{id}");
        }

        public async Task<CategoriaProducto> CreateAsync(CategoriaProducto entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CategoriasProducto", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoriaProducto>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, CategoriaProducto entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/CategoriasProducto/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/CategoriasProducto/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
