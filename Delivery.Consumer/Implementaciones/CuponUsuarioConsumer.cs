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

        public async Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            var pid = pedidoId.HasValue ? pedidoId.ToString() : "null";
            return await _httpClient.GetFromJsonAsync<CuponUsuario>($"api/CuponesUsuarios/{cuponId}/{usuarioId}/{pid}");
        }

        public async Task<CuponUsuario> CreateAsync(CuponUsuario entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CuponesUsuarios", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CuponUsuario>() ?? entity;
        }

        public async Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            var pid = pedidoId.HasValue ? pedidoId.ToString() : "null";
            var response = await _httpClient.DeleteAsync($"api/CuponesUsuarios/{cuponId}/{usuarioId}/{pid}");
            return response.IsSuccessStatusCode;
        }
    }
}
