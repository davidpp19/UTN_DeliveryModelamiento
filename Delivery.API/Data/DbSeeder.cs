using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(DeliveryDbContext context, ISeguridadService seguridadService)
        {
            await context.Database.MigrateAsync();

            var adminRole = await SeedRolesAsync(context);
            var (admin, cliente, repartidores) = await SeedUsuariosAsync(context, seguridadService);
            var restaurantes = await SeedRestaurantesYProductosAsync(context);
            await SeedCuponesAsync(context, restaurantes);
            await SeedVehiculosYRepartidoresAsync(context, repartidores);
            await SeedPedidosAsync(context, cliente, restaurantes, repartidores);
        }

        private static async Task<Rol> SeedRolesAsync(DeliveryDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Rol { Nombre = "Admin" },
                    new Rol { Nombre = "Restaurante" },
                    new Rol { Nombre = "Repartidor" },
                    new Rol { Nombre = "Cliente" }
                );
                await context.SaveChangesAsync();
            }
            return await context.Roles.FirstAsync(r => r.Nombre == "Admin");
        }

        private static async Task<(Usuario admin, Usuario cliente, List<Usuario> repartidores)> SeedUsuariosAsync(DeliveryDbContext context, ISeguridadService seguridadService)
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Nombre == "Admin");
            var clienteRole = await context.Roles.FirstAsync(r => r.Nombre == "Cliente");
            var repartidorRole = await context.Roles.FirstAsync(r => r.Nombre == "Repartidor");

            if (!await context.Usuarios.AnyAsync(u => u.Email == "admin@rayoexpres.com"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = "Administrador", Apellidos = "Principal",
                    Email = "admin@rayoexpres.com", Telefono = "0999999999",
                    PasswordHash = seguridadService.HashearPassword("Admin123*"),
                    RolId = adminRole.Id, TipoUsuario = TipoUsuarioEnum.Administrador,
                    Activo = true, CreadoEn = DateTime.UtcNow
                });
            }

            if (!await context.Usuarios.AnyAsync(u => u.Email == "cliente@rayoexpres.com"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = "Cliente", Apellidos = "Prueba Ibarra",
                    Email = "cliente@rayoexpres.com", Telefono = "0988888888",
                    PasswordHash = seguridadService.HashearPassword("Cliente123*"),
                    RolId = clienteRole.Id, TipoUsuario = TipoUsuarioEnum.Cliente,
                    Activo = true, CreadoEn = DateTime.UtcNow
                });
            }

            for (int i = 1; i <= 3; i++)
            {
                string email = $"repartidor{i}@rayoexpres.com";
                if (!await context.Usuarios.AnyAsync(u => u.Email == email))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre = $"Repartidor {i}", Apellidos = "RayoExpres",
                        Email = email, Telefono = $"097777770{i}",
                        PasswordHash = seguridadService.HashearPassword("Repartidor123*"),
                        RolId = repartidorRole.Id, TipoUsuario = TipoUsuarioEnum.Repartidor,
                        Activo = true, CreadoEn = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
            
            var admin = await context.Usuarios.FirstAsync(u => u.Email == "admin@rayoexpres.com");
            var cliente = await context.Usuarios.FirstAsync(u => u.Email == "cliente@rayoexpres.com");
            var repartidores = await context.Usuarios.Where(u => u.Email.StartsWith("repartidor")).ToListAsync();
            
            return (admin, cliente, repartidores);
        }

        private static async Task<List<CategoriaProducto>> SeedCategoriasAsync(DeliveryDbContext context, Restaurante restaurante)
        {
            var categoriasNombres = new[] { "Hamburguesas", "Pizza", "Pollo", "Comida Típica", "Parrilladas", "Sushi", "Mexicana", "Italiana", "Cafetería", "Postres", "Bebidas", "Ensaladas" };
            var catsToInsert = new List<CategoriaProducto>();

            foreach (var nombre in categoriasNombres)
            {
                if (!await context.CategoriasProducto.AnyAsync(c => c.Nombre == nombre && c.RestauranteId == restaurante.Id))
                {
                    catsToInsert.Add(new CategoriaProducto { RestauranteId = restaurante.Id, Nombre = nombre, Descripcion = $"Deliciosa categoría de {nombre}", CreadoEn = DateTime.UtcNow });
                }
            }

            if (catsToInsert.Any())
            {
                context.CategoriasProducto.AddRange(catsToInsert);
                await context.SaveChangesAsync();
            }

            return await context.CategoriasProducto.Where(c => c.RestauranteId == restaurante.Id).ToListAsync();
        }

        private static async Task<List<Restaurante>> SeedRestaurantesYProductosAsync(DeliveryDbContext context)
        {
            var restauranteRole = await context.Roles.FirstAsync(r => r.Nombre == "Restaurante");
            var resData = new List<(string Nombre, string Categoria, string Desc, decimal Envio, string LogoUrl)>
            {
                ("Parrilla Ibarra", "Parrilladas", "Los mejores cortes de carne asados a la parrilla.", 1.50m, "https://images.unsplash.com/photo-1544025162-8111142109b0?auto=format&fit=crop&w=400&q=80"),
                ("El Rincón Imbabureño", "Comida Típica", "Sabor tradicional de Imbabura.", 2.00m, "https://images.unsplash.com/photo-1565299507177-b0ac66763828?auto=format&fit=crop&w=400&q=80"),
                ("Pizza Andina", "Pizza", "Pizzas artesanales con toque andino.", 1.00m, "https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=400&q=80"),
                ("Sabor Norteño", "Mexicana", "Auténtica comida del norte y tacos.", 1.25m, "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?auto=format&fit=crop&w=400&q=80"),
                ("Café Laguna", "Cafetería", "Café premium frente a Yahuarcocha.", 0.00m, "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=400&q=80"),
                ("Sushi Sakura Ibarra", "Sushi", "El mejor sushi de la ciudad blanca.", 2.50m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=400&q=80"),
                ("Pollos del Valle", "Pollo", "Pollo broaster crujiente y jugoso.", 1.00m, "https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?auto=format&fit=crop&w=400&q=80"),
                ("Hamburguesas Volcán", "Hamburguesas", "Hamburguesas explosivas gigantes.", 1.50m, "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=400&q=80"),
                ("Tacos La Merced", "Mexicana", "Tacos al pastor y burritos tradicionales.", 1.25m, "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&w=400&q=80"),
                ("Pastas San Miguel", "Italiana", "Pastas caseras y lasagna al horno.", 1.75m, "https://images.unsplash.com/photo-1621996311239-5a50785023fa?auto=format&fit=crop&w=400&q=80"),
                ("La Esquina del Hornado", "Comida Típica", "Hornado completo con llapingachos.", 1.50m, "https://images.unsplash.com/photo-1541832676-9b763b0239ab?auto=format&fit=crop&w=400&q=80"),
                ("Green Bowl Ibarra", "Ensaladas", "Opciones saludables y bowls nutritivos.", 1.00m, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=400&q=80")
            };

            foreach (var rData in resData)
            {
                if (!await context.Restaurantes.AnyAsync(r => r.Nombre == rData.Nombre))
                {
                    // Crear usuario restaurante
                    var rUser = new Usuario
                    {
                        Nombre = rData.Nombre, Apellidos = "Admin",
                        Email = rData.Nombre.Replace(" ", "").ToLower() + "@rayoexpres.com", Telefono = "0900000000",
                        PasswordHash = "$2a$11$0Ff.zW.Z..z.z.z.z.z.z.z.z.z.z.z.z.z.z.z.z.z.z.z.z", // Generic hash for 'Restaurante123*'
                        RolId = restauranteRole.Id, TipoUsuario = TipoUsuarioEnum.Restaurante,
                        Activo = true, CreadoEn = DateTime.UtcNow
                    };
                    context.Usuarios.Add(rUser);
                    await context.SaveChangesAsync();

                    var restaurante = new Restaurante
                    {
                        Nombre = rData.Nombre, Descripcion = rData.Desc,
                        Categoria = rData.Categoria, Calle = "Av. Mariano Acosta",
                        Ciudad = "Ibarra", Telefono = "062000000",
                        Email = rUser.Email,
                        Estado = EstadoRestauranteEnum.Aprobado, Abierto = true,
                        LogoUrl = rData.LogoUrl,
                        CostoEnvioBase = rData.Envio,
                        CreadoPor = rUser.Id,
                        CreadoEn = DateTime.UtcNow
                    };
                    context.Restaurantes.Add(restaurante);
                    await context.SaveChangesAsync();

                    var categorias = await SeedCategoriasAsync(context, restaurante);
                    await SeedProductosParaRestauranteAsync(context, restaurante, categorias);
                }
            }

            return await context.Restaurantes.ToListAsync();
        }

        private static async Task SeedProductosParaRestauranteAsync(DeliveryDbContext context, Restaurante restaurante, List<CategoriaProducto> categorias)
        {
            var cat = categorias.FirstOrDefault(c => c.Nombre == restaurante.Categoria) ?? categorias.First();
            var rng = new Random();
            var productosBase = new List<(string Nombre, decimal Precio)>
            {
                ("Hamburguesa Clásica", 5.99m), ("Hamburguesa Doble", 7.99m), ("Pizza Familiar", 15.50m),
                ("Pizza Personal", 6.75m), ("Hornado Completo", 8.50m), ("Fritada Imbabureña", 9.25m),
                ("Pollo Broaster", 6.99m), ("Alitas BBQ", 8.75m), ("California Roll", 10.99m),
                ("Tacos al Pastor", 7.50m), ("Lasagna", 9.90m), ("Capuccino", 2.75m),
                ("Cheesecake", 3.95m), ("Limonada Natural", 2.00m), ("Coca Cola 500ml", 1.50m),
                ("Papas Fritas", 2.50m), ("Nuggets de Pollo", 4.50m), ("Ensalada César", 5.50m)
            };

            // Tomar 10-15 productos aleatorios
            var productosSeleccionados = productosBase.OrderBy(x => rng.Next()).Take(rng.Next(10, 16)).ToList();

            foreach (var p in productosSeleccionados)
            {
                context.Productos.Add(new Producto
                {
                    RestauranteId = restaurante.Id, CategoriaId = cat.Id,
                    Nombre = p.Nombre, Descripcion = $"Delicioso {p.Nombre} preparado al instante.",
                    Precio = p.Precio, Disponible = true,
                    TiempoPreparacion = rng.Next(10, 25),
                    ImagenUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=300&q=80",
                    CreadoEn = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedCuponesAsync(DeliveryDbContext context, List<Restaurante> restaurantes)
        {
            if (!await context.Cupones.AnyAsync())
            {
                context.Cupones.AddRange(
                    new Cupon { Codigo = "BIENVENIDO10", TipoDescuento = TipoDescuentoEnum.Porcentaje, ValorDescuento = 10, FechaInicio = DateTime.UtcNow, FechaFin = DateTime.UtcNow.AddMonths(3), Activo = true },
                    new Cupon { Codigo = "ENVIOGRATIS", TipoDescuento = TipoDescuentoEnum.MontoFijo, ValorDescuento = 2.50m, FechaInicio = DateTime.UtcNow, FechaFin = DateTime.UtcNow.AddMonths(1), Activo = true },
                    new Cupon { Codigo = "RAYO20", TipoDescuento = TipoDescuentoEnum.Porcentaje, ValorDescuento = 20, PedidoMinimo = 25.00m, FechaInicio = DateTime.UtcNow, FechaFin = DateTime.UtcNow.AddMonths(6), Activo = true }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedVehiculosYRepartidoresAsync(DeliveryDbContext context, List<Usuario> repartidores)
        {
            if (!await context.Vehiculos.AnyAsync())
            {
                string[] tipos = { "Motocicleta", "Bicicleta", "Automóvil" };
                for (int i = 0; i < repartidores.Count; i++)
                {
                    var repId = repartidores[i].Id;
                    if (!await context.Repartidores.AnyAsync(r => r.UsuarioId == repId))
                    {
                        var repartidor = new Repartidor { UsuarioId = repId, Disponible = true, CalificacionPromedio = 4.8m };
                        context.Repartidores.Add(repartidor);
                        await context.SaveChangesAsync();

                        context.Vehiculos.Add(new Vehiculo
                        {
                            RepartidorId = repartidor.UsuarioId, TipoVehiculo = TipoVehiculoEnum.Motocicleta,
                            Marca = "Generica", Modelo = "2020", Placa = $"ABC-{100+i}",
                            Color = "Negro"
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private static async Task SeedPedidosAsync(DeliveryDbContext context, Usuario cliente, List<Restaurante> restaurantes, List<Usuario> repartidores)
        {
            if (!await context.Pedidos.AnyAsync())
            {
                // Asegurar al menos una dirección para el cliente
                var direccion = await context.Direcciones.FirstOrDefaultAsync(d => d.UsuarioId == cliente.Id);
                if (direccion == null)
                {
                    direccion = new Direccion
                    {
                        UsuarioId = cliente.Id, Calle = "Calle Ficticia", Numero = "123",
                        Ciudad = "Ibarra", EsPrincipal = true, CreadoEn = DateTime.UtcNow
                    };
                    context.Direcciones.Add(direccion);
                    await context.SaveChangesAsync();
                }

                var rng = new Random();
                var estados = Enum.GetValues(typeof(EstadoPedidoEnum)).Cast<EstadoPedidoEnum>().ToList();

                for (int i = 0; i < 5; i++)
                {
                    var res = restaurantes[rng.Next(restaurantes.Count)];
                    var rep = await context.Repartidores.FirstOrDefaultAsync(r => r.UsuarioId == repartidores[rng.Next(repartidores.Count)].Id);
                    
                    var pedido = new Pedido
                    {
                        UsuarioId = cliente.Id, RestauranteId = res.Id, RepartidorId = rep?.UsuarioId,
                        DireccionEntregaId = direccion.Id,
                        Subtotal = 15.00m, CostoEnvio = res.CostoEnvioBase, Total = 15.00m + res.CostoEnvioBase,
                        EstadoPedido = estados[rng.Next(estados.Count)],
                        TipoMetodoPago = TipoMetodoPagoEnum.Efectivo,
                        FechaPedido = DateTime.UtcNow.AddDays(-rng.Next(1, 30))
                    };
                    context.Pedidos.Add(pedido);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
