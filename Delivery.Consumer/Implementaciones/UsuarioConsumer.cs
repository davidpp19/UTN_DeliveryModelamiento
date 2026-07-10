using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class UsuarioConsumer : IUsuarioConsumer
    {
        private readonly HttpClient _httpClient;

        public UsuarioConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Usuario>>("api/Usuarios") ?? new List<Usuario>();
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"api/Usuarios/{id}");
        }

        public async Task<Usuario?> CreateAsync(Usuario entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", entity);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task<bool> UpdateAsync(long id, Usuario entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Delivery.Modelos.Entidades.Notificacion>> GetNotificacionesAsync(long id)
        {
            var response = await _httpClient.GetAsync($"api/Usuarios/{id}/notificaciones");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Delivery.Modelos.Entidades.Notificacion>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? new List<Delivery.Modelos.Entidades.Notificacion>();
            }
            return new List<Delivery.Modelos.Entidades.Notificacion>();
        }
    }
}
