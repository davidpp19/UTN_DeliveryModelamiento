using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class AuditoriaConsumer : IAuditoriaConsumer
    {
        private readonly HttpClient _httpClient;

        public AuditoriaConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<RegistroAuditoria>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/Auditorias");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<RegistroAuditoria>>() ?? new List<RegistroAuditoria>();
            }
            return new List<RegistroAuditoria>();
        }

        public async Task<RegistroAuditoria?> GetByIdAsync(long id)
        {
            var response = await _httpClient.GetAsync($"api/Auditorias/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RegistroAuditoria>();
            }
            return null;
        }

        public async Task<RegistroAuditoria> CreateAsync(RegistroAuditoria entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auditorias", entity);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RegistroAuditoria>() ?? entity;
            }
            return entity;
        }
    }
}
