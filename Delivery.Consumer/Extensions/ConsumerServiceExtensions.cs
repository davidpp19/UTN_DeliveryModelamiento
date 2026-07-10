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
            // Registrar IHttpContextAccessor y AuthTokenHandler
            services.AddHttpContextAccessor();
            services.AddTransient<AuthTokenHandler>();

            // Configurar cada HttpClient con el Handler de Autenticación
            services.AddHttpClient<IRolConsumer, RolConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IUsuarioConsumer, UsuarioConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IDireccionConsumer, DireccionConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IRestauranteConsumer, RestauranteConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<ICategoriaProductoConsumer, CategoriaProductoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IProductoConsumer, ProductoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IRepartidorConsumer, RepartidorConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IVehiculoConsumer, VehiculoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IUbicacionActualRepartidorConsumer, UbicacionActualRepartidorConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IAuthConsumer, AuthConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IPedidoConsumer, PedidoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IDetallePedidoConsumer, DetallePedidoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IPagoConsumer, PagoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IResenaConsumer, ResenaConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<ICuponConsumer, CuponConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<ICuponUsuarioConsumer, CuponUsuarioConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IFavoritoConsumer, FavoritoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IAuditoriaConsumer, AuditoriaConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IDashboardConsumer, DashboardConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<ICarritoConsumer, CarritoConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();
            services.AddHttpClient<IAdminAprobacionesConsumer, AdminAprobacionesConsumer>(configureClient).AddHttpMessageHandler<AuthTokenHandler>();

            return services;
        }
    }
}
