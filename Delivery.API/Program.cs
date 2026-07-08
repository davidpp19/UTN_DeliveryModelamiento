using Delivery.Modelos;
using Delivery.Servicios.Interfaces;
using Delivery.Servicios.Implementaciones;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Server=127.0.0.1;Port=5432;Database=UTN_DeliveryModelamiento;User Id=postgres;Password=Megustanlosgatos:3S;";
var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<TipoUsuarioEnum>();
dataSourceBuilder.MapEnum<EstadoRestauranteEnum>();
dataSourceBuilder.MapEnum<EstadoAprobacionEnum>();
dataSourceBuilder.MapEnum<TipoVehiculoEnum>();
dataSourceBuilder.MapEnum<EstadoPedidoEnum>();
dataSourceBuilder.MapEnum<TipoMetodoPagoEnum>();
dataSourceBuilder.MapEnum<EstadoPagoEnum>();
dataSourceBuilder.MapEnum<TipoDescuentoEnum>();
dataSourceBuilder.MapEnum<TipoAccionAuditoriaEnum>();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<IRestauranteService, RestauranteService>();
builder.Services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IRepartidorService, RepartidorService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IUbicacionActualRepartidorService, UbicacionActualRepartidorService>();
builder.Services.AddScoped<IHistorialAsignacionesRepartidorService, HistorialAsignacionesRepartidorService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IDetallePedidoService, DetallePedidoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IResenaService, ResenaService>();
builder.Services.AddScoped<ICuponService, CuponService>();
builder.Services.AddScoped<ICuponUsuarioService, CuponUsuarioService>();
builder.Services.AddScoped<IFavoritoService, FavoritoService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IPasarelaPagoService, PasarelaPagoService>();
builder.Services.AddScoped<IGeolocalizacionService, GeolocalizacionService>();
builder.Services.AddScoped<IBusquedaService, BusquedaService>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();

// Configuración de JWT
var keyConfig = builder.Configuration["Jwt:Key"] ?? "ClaveSuperSecretaParaDesarrolloQueDeberiaEstarEnAppsettings.123456789";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "DeliveryAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "DeliveryClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyConfig))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<Delivery.API.Middlewares.ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
