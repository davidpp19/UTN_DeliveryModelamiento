using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class FavoritoConsumer : IFavoritoConsumer
    {
        private readonly HttpClient _httpClient;

        public FavoritoConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Favorito>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Favorito>>("api/Favoritos") ?? new List<Favorito>();
        }

        public async Task<Favorito?> GetByIdsAsync(long usuarioId, long restauranteId)
        {
            return await _httpClient.GetFromJsonAsync<Favorito>($"api/Favoritos/{usuarioId}/{restauranteId}");
        }

        public async Task<Favorito> CreateAsync(Favorito entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Favoritos", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Favorito>() ?? entity;
        }

        public async Task<bool> DeleteAsync(long usuarioId, long restauranteId)
        {
            var response = await _httpClient.DeleteAsync($"api/Favoritos/{usuarioId}/{restauranteId}");
            return response.IsSuccessStatusCode;
        }
    }
}
