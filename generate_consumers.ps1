$entities = @(
    @{ Name = 'Rol'; Route = 'Roles'; Type = 'long' },
    @{ Name = 'Usuario'; Route = 'Usuarios'; Type = 'long' },
    @{ Name = 'Direccion'; Route = 'Direcciones'; Type = 'long' },
    @{ Name = 'Restaurante'; Route = 'Restaurantes'; Type = 'long' },
    @{ Name = 'CategoriaProducto'; Route = 'CategoriasProducto'; Type = 'long' },
    @{ Name = 'Producto'; Route = 'Productos'; Type = 'long' },
    @{ Name = 'Repartidor'; Route = 'Repartidores'; Type = 'long' },
    @{ Name = 'Vehiculo'; Route = 'Vehiculos'; Type = 'long' },
    @{ Name = 'UbicacionActualRepartidor'; Route = 'UbicacionesActuales'; Type = 'long' },
    @{ Name = 'HistorialAsignacionesRepartidor'; Route = 'HistorialAsignaciones'; Type = 'long' },
    @{ Name = 'Pedido'; Route = 'Pedidos'; Type = 'long' },
    @{ Name = 'DetallePedido'; Route = 'DetallesPedido'; Type = 'long' },
    @{ Name = 'Pago'; Route = 'Pagos'; Type = 'long' },
    @{ Name = 'Resena'; Route = 'Resenas'; Type = 'long' },
    @{ Name = 'Cupon'; Route = 'Cupones'; Type = 'long' }
)

New-Item -ItemType Directory -Force -Path 'Delivery.Consumer\Interfaces'
New-Item -ItemType Directory -Force -Path 'Delivery.Consumer\Implementaciones'

foreach ($e in $entities) {
    $name = $e.Name
    $route = $e.Route
    $type = $e.Type

    $interfaceContent = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface I${name}Consumer
    {
        Task<IEnumerable<${name}>> GetAllAsync();
        Task<${name}?> GetByIdAsync(${type} id);
        Task<${name}> CreateAsync(${name} entity);
        Task<bool> UpdateAsync(${type} id, ${name} entity);
        Task<bool> DeleteAsync(${type} id);
    }
}
"@
    Set-Content -Path "Delivery.Consumer\Interfaces\I${name}Consumer.cs" -Value $interfaceContent

    $classContent = @"
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class ${name}Consumer : I${name}Consumer
    {
        private readonly HttpClient _httpClient;

        public ${name}Consumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<${name}>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<${name}>>("api/${route}") ?? new List<${name}>();
        }

        public async Task<${name}?> GetByIdAsync(${type} id)
        {
            return await _httpClient.GetFromJsonAsync<${name}>($"api/${route}/{id}");
        }

        public async Task<${name}> CreateAsync(${name} entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/${route}", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<${name}>() ?? entity;
        }

        public async Task<bool> UpdateAsync(${type} id, ${name} entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/${route}/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(${type} id)
        {
            var response = await _httpClient.DeleteAsync($"api/${route}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
"@
    Set-Content -Path "Delivery.Consumer\Implementaciones\${name}Consumer.cs" -Value $classContent
}

# Generar CuponUsuarioConsumer (Llave compuesta)
$cuInterface = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface ICuponUsuarioConsumer
    {
        Task<IEnumerable<CuponUsuario>> GetAllAsync();
        Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId);
        Task<CuponUsuario> CreateAsync(CuponUsuario entity);
        Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId);
    }
}
"@
Set-Content -Path "Delivery.Consumer\Interfaces\ICuponUsuarioConsumer.cs" -Value $cuInterface

$cuClass = @"
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.Consumer.Implementaciones
{
    public class CuponUsuarioConsumer : ICuponUsuarioConsumer
    {
        private readonly HttpClient _httpClient;

        public CuponUsuarioConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CuponUsuario>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CuponUsuario>>("api/CuponesUsuarios") ?? new List<CuponUsuario>();
        }

        public async Task<CuponUsuario?> GetByIdsAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            var pid = pedidoId.HasValue ? pedidoId.ToString() : "null";
            return await _httpClient.GetFromJsonAsync<CuponUsuario>($"api/CuponesUsuarios/{cuponId}/{usuarioId}/{pid}");
        }

        public async Task<CuponUsuario> CreateAsync(CuponUsuario entity)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CuponesUsuarios", entity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CuponUsuario>() ?? entity;
        }

        public async Task<bool> DeleteAsync(long cuponId, long usuarioId, long? pedidoId)
        {
            var pid = pedidoId.HasValue ? pedidoId.ToString() : "null";
            var response = await _httpClient.DeleteAsync($"api/CuponesUsuarios/{cuponId}/{usuarioId}/{pid}");
            return response.IsSuccessStatusCode;
        }
    }
}
"@
Set-Content -Path "Delivery.Consumer\Implementaciones\CuponUsuarioConsumer.cs" -Value $cuClass

# Generar FavoritoConsumer (Llave compuesta)
$favInterface = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IFavoritoConsumer
    {
        Task<IEnumerable<Favorito>> GetAllAsync();
        Task<Favorito?> GetByIdsAsync(long usuarioId, long restauranteId);
        Task<Favorito> CreateAsync(Favorito entity);
        Task<bool> DeleteAsync(long usuarioId, long restauranteId);
    }
}
"@
Set-Content -Path "Delivery.Consumer\Interfaces\IFavoritoConsumer.cs" -Value $favInterface

$favClass = @"
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
"@
Set-Content -Path "Delivery.Consumer\Implementaciones\FavoritoConsumer.cs" -Value $favClass
