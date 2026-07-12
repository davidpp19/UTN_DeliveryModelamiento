using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class CarritoConsumer : ICarritoConsumer
    {
        private readonly HttpClient _httpClient;

        public CarritoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Carrito?> GetCarritoAsync(long usuarioId)
        {
            var response = await _httpClient.GetAsync($"api/Carrito/{usuarioId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Carrito>();
            }
            return null;
        }

        public async Task<Carrito?> AgregarProductoAsync(AgregarAlCarritoDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Carrito/agregar", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Carrito>();
            }
            return null;
        }

        public async Task<bool> QuitarProductoAsync(long usuarioId, long carritoItemId)
        {
            var response = await _httpClient.DeleteAsync($"api/Carrito/{usuarioId}/quitar/{carritoItemId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VaciarCarritoAsync(long usuarioId)
        {
            var response = await _httpClient.DeleteAsync($"api/Carrito/{usuarioId}/vaciar");
            return response.IsSuccessStatusCode;
        }
    }
}
