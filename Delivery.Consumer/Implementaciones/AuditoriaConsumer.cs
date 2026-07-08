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
            return await _httpClient.GetFromJsonAsync<IEnumerable<RegistroAuditoria>>("api/Auditorias") ?? new List<RegistroAuditoria>();
        }

        public async Task<RegistroAuditoria?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<RegistroAuditoria>($"api/Auditorias/{id}");
        }

        public async Task<RegistroAuditoria> CreateAsync(RegistroAuditoria entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auditorias", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RegistroAuditoria>() ?? entity;
        }
    }
}
