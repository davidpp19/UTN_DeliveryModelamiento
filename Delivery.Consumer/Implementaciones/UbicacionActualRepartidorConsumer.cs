using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class UbicacionActualRepartidorConsumer : IUbicacionActualRepartidorConsumer
    {
        private readonly HttpClient _httpClient;

        public UbicacionActualRepartidorConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UbicacionActualRepartidor>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UbicacionActualRepartidor>>("api/UbicacionesActuales") ?? new List<UbicacionActualRepartidor>();
        }

        public async Task<UbicacionActualRepartidor?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<UbicacionActualRepartidor>($"api/UbicacionesActuales/{id}");
        }

        public async Task<UbicacionActualRepartidor> CreateAsync(UbicacionActualRepartidor entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/UbicacionesActuales", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UbicacionActualRepartidor>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, UbicacionActualRepartidor entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/UbicacionesActuales/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/UbicacionesActuales/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
