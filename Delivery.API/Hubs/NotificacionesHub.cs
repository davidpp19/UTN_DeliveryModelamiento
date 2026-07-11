using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Delivery.API.Hubs
{
    public class NotificacionesHub : Hub
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificacionesHub(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // El cliente (MVC o JS) se unirá a un grupo específico dependiendo de su rol y su ID.

        public async Task UnirseGrupoRestaurante(string restauranteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Restaurante_{restauranteId}");
        }

        public async Task UnirseGrupoRepartidoresLibres()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "RepartidoresLibres");
        }
        
        public async Task AbandonarGrupoRepartidoresLibres()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RepartidoresLibres");
        }

        public async Task UnirseGrupoCliente(string clienteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Cliente_{clienteId}");
        }

        public async Task UnirseGrupoPedido(string pedidoId)
        {
            // Usado para tracking en vivo del mapa
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Pedido_{pedidoId}");
        }

        public async Task EnviarUbicacionRepartidor(string pedidoId, double latitud, double longitud, long repartidorId)
        {
            // El repartidor envía su ubicación y se transmite a los interesados (ej. Cliente)
            await Clients.Group($"Pedido_{pedidoId}").SendAsync("RecibirUbicacionRepartidor", latitud, longitud);

            // Persistencia en Base de Datos de la última ubicación conocida
            using (var scope = _scopeFactory.CreateScope())
            {
                var ubicacionService = scope.ServiceProvider.GetRequiredService<Delivery.Servicios.Interfaces.IUbicacionActualRepartidorService>();
                var ubicacion = await ubicacionService.GetByIdAsync(repartidorId);
                
                if (ubicacion != null)
                {
                    ubicacion.Latitud = (decimal)latitud;
                    ubicacion.Longitud = (decimal)longitud;
                    ubicacion.ActualizadoEn = System.DateTime.UtcNow;
                    await ubicacionService.UpdateAsync(ubicacion);
                }
                else
                {
                    await ubicacionService.CreateAsync(new Delivery.Modelos.Entidades.UbicacionActualRepartidor
                    {
                        RepartidorId = repartidorId,
                        Latitud = (decimal)latitud,
                        Longitud = (decimal)longitud,
                        ActualizadoEn = System.DateTime.UtcNow
                    });
                }
            }
        }
    }
}
