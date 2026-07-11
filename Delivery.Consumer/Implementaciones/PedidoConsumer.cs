using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class PedidoConsumer : IPedidoConsumer
    {
        private readonly HttpClient _httpClient;

        public PedidoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Pedido>>("api/Pedidos") ?? new List<Pedido>();
        }

        public async Task<Pedido?> GetByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Pedido>($"api/Pedidos/{id}");
        }

        public async Task<Pedido> CreateAsync(Pedido entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Pedidos", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Pedido>() ?? entity;
        }

        public async Task<bool> UpdateAsync(long id, Pedido entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Pedidos/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Pedidos/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Pedido?> ActualizarEstadoRestauranteAsync(long id, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long restauranteId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Pedidos/{id}/estado-restaurante?restauranteId={restauranteId}", nuevoEstado);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Pedido>();
            return null;
        }

        public async Task<Pedido?> ActualizarEstadoRepartidorAsync(long id, Delivery.Modelos.Enums.EstadoPedidoEnum nuevoEstado, long repartidorId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Pedidos/{id}/estado-repartidor?repartidorId={repartidorId}", nuevoEstado);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Pedido>();
            return null;
        }

        public async Task<Pedido?> AsignarPedidoAsync(long id, long repartidorId)
        {
            // Pasamos null como body ya que no requiere un payload en el body
            var response = await _httpClient.PutAsJsonAsync($"api/Pedidos/{id}/asignar-repartidor?repartidorId={repartidorId}", new object());
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Pedido>();
            return null;
        }

        /// <summary>
        /// Llama al endpoint que crea el Pedido real desde el carrito de sesión.
        /// Solo se llama cuando el usuario presiona "Confirmar Compra".
        /// </summary>
        public async Task<Pedido?> CrearDesdeCarritoAsync(long usuarioId, long direccionId, Delivery.Modelos.Enums.TipoMetodoPagoEnum metodoPago, CarritoSesionDto carrito)
        {
            var url = $"api/Pedidos/crear-desde-carrito?usuarioId={usuarioId}&direccionId={direccionId}&metodoPago={(int)metodoPago}";
            var response = await _httpClient.PostAsJsonAsync(url, carrito);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Pedido>();
            return null;
        }
    }
}

