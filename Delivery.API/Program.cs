using Delivery.Modelos;
using Delivery.Servicios.Interfaces;
using Delivery.Servicios.Implementaciones;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos.Enums;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
