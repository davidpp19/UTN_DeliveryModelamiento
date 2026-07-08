using System;
using Microsoft.Extensions.DependencyInjection;
using Delivery.Consumer.Interfaces;
using Delivery.Consumer.Implementaciones;

namespace Delivery.Consumer.Extensions
{
    public static class ConsumerServiceExtensions
    {
        public static IServiceCollection AddDeliveryConsumers(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient<IRolConsumer, RolConsumer>(configureClient);
            services.AddHttpClient<IUsuarioConsumer, UsuarioConsumer>(configureClient);
            services.AddHttpClient<IDireccionConsumer, DireccionConsumer>(configureClient);
            services.AddHttpClient<IRestauranteConsumer, RestauranteConsumer>(configureClient);
            services.AddHttpClient<ICategoriaProductoConsumer, CategoriaProductoConsumer>(configureClient);
            services.AddHttpClient<IProductoConsumer, ProductoConsumer>(configureClient);
            services.AddHttpClient<IRepartidorConsumer, RepartidorConsumer>(configureClient);
            services.AddHttpClient<IVehiculoConsumer, VehiculoConsumer>(configureClient);
            services.AddHttpClient<IUbicacionActualRepartidorConsumer, UbicacionActualRepartidorConsumer>(configureClient);
            services.AddHttpClient<IHistorialAsignacionesRepartidorConsumer, HistorialAsignacionesRepartidorConsumer>(configureClient);
            services.AddHttpClient<IPedidoConsumer, PedidoConsumer>(configureClient);
            services.AddHttpClient<IDetallePedidoConsumer, DetallePedidoConsumer>(configureClient);
            services.AddHttpClient<IPagoConsumer, PagoConsumer>(configureClient);
            services.AddHttpClient<IResenaConsumer, ResenaConsumer>(configureClient);
            services.AddHttpClient<ICuponConsumer, CuponConsumer>(configureClient);
            services.AddHttpClient<ICuponUsuarioConsumer, CuponUsuarioConsumer>(configureClient);
            services.AddHttpClient<IFavoritoConsumer, FavoritoConsumer>(configureClient);

            return services;
        }
    }
}
