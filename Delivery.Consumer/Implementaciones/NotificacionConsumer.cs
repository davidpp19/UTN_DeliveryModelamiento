using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Delivery.Modelos.DTOs;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class NotificacionConsumer : INotificacionConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificacionConsumer> _logger;

        public NotificacionConsumer(HttpClient httpClient, IConfiguration configuration, ILogger<NotificacionConsumer> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7087/api/");
        }

        public async Task<IEnumerable<NotificacionDto>?> GetMisNotificacionesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Notificaciones");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<IEnumerable<NotificacionDto>>(content, options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones");
            }
            return null;
        }

        public async Task<bool> MarcarComoLeidaAsync(long notificacionId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"Notificaciones/{notificacionId}/leer", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar notificación como leída");
                return false;
            }
        }
    }
}
