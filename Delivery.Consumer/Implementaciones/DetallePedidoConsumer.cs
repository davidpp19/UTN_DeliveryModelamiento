using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class DetallePedidoConsumer : IDetallePedidoConsumer
    {
        private readonly HttpClient _httpClient;

        public DetallePedidoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DetallePedido>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DetallePedido>>("api/DetallesPedido") ?? new List<DetallePedido>();
        }

        public async Task<DetallePedido?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<DetallePedido>($"api/DetallesPedido/{id}");
        }

        public async Task<DetallePedido> CreateAsync(DetallePedido entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/DetallesPedido", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DetallePedido>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, DetallePedido entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/DetallesPedido/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/DetallesPedido/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
