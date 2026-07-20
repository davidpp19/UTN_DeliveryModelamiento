using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class AuthConsumer : IAuthConsumer
    {
        private readonly HttpClient _httpClient;

        public AuthConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorObj = System.Text.Json.JsonDocument.Parse(errorContent);
            string message = "Credenciales incorrectas.";
            if (errorObj.RootElement.TryGetProperty("message", out var msgProp))
            {
                message = msgProp.GetString() ?? message;
            }
            
            throw new System.Exception(message);
        }

        public async Task<bool> RecuperarPasswordAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/recuperar-password", email);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthResponseDto?> RegistroRepartidorAsync(RegistroRepartidorDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/registro-repartidor", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"API Error ({response.StatusCode}): {errorContent}");
        }

        public async Task<AuthResponseDto?> RegistroRestauranteAsync(RegistroRestauranteDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/registro-restaurante", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"API Error ({response.StatusCode}): {errorContent}");
        }
    }
}
