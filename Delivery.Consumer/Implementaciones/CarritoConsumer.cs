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

        public async Task<Pedido?> GetCarritoAsync(long usuarioId)
        {
            var response = await _httpClient.GetAsync($"api/Carrito/{usuarioId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Pedido>();
            }
            return null;
        }

        public async Task<Pedido?> AgregarProductoAsync(AgregarAlCarritoDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Carrito/agregar", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Pedido>();
            }
            return null;
        }

        public async Task<bool> QuitarProductoAsync(long usuarioId, long detallePedidoId)
        {
            var response = await _httpClient.DeleteAsync($"api/Carrito/{usuarioId}/quitar/{detallePedidoId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Pedido?> ConfirmarCarritoAsync(long usuarioId, long direccionId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Carrito/{usuarioId}/confirmar", direccionId);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Pedido>();
            }
            return null;
        }
    }
}
