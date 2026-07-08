using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class VehiculoConsumer : IVehiculoConsumer
    {
        private readonly HttpClient _httpClient;

        public VehiculoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Vehiculo>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Vehiculo>>("api/Vehiculos") ?? new List<Vehiculo>();
        }

        public async Task<Vehiculo?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Vehiculo>($"api/Vehiculos/{id}");
        }

        public async Task<Vehiculo> CreateAsync(Vehiculo entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Vehiculos", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehiculo>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Vehiculo entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Vehiculos/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Vehiculos/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
