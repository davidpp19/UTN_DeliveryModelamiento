using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Delivery.API.Hubs
{
    public class NotificacionesHub : Hub
    {
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

        public async Task EnviarUbicacionRepartidor(string pedidoId, double latitud, double longitud)
        {
            // El repartidor envía su ubicación y se transmite a los interesados (ej. Cliente)
            await Clients.Group($"Pedido_{pedidoId}").SendAsync("RecibirUbicacionRepartidor", latitud, longitud);
        }
    }
}
