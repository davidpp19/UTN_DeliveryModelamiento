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

            await SeedRolesAsync(context);
            var (cliente, repartidores) = await SeedUsuariosAsync(context, seguridadService);
            var restaurantes = await SeedRestaurantesYProductosAsync(context, seguridadService);
            await SeedCuponesAsync(context);
            await SeedRepartidoresYVehiculosAsync(context, repartidores);
            await SeedDireccionYPedidosAsync(context, cliente, restaurantes, repartidores);
        }

        // =====================================================================
        // ROLES
        // =====================================================================
        private static async Task SeedRolesAsync(DeliveryDbContext context)
        {
            var ahora = DateTime.UtcNow;
            var rolesExistentes = await context.Roles.Select(r => r.Nombre).ToListAsync();

            var rolesNuevos = new List<string> { "Admin", "Restaurante", "Repartidor", "Cliente" }
                .Where(n => !rolesExistentes.Contains(n))
                .Select(n => new Rol { Nombre = n, CreadoEn = ahora })
                .ToList();

            if (rolesNuevos.Any())
            {
                context.Roles.AddRange(rolesNuevos);
                await context.SaveChangesAsync();
            }
        }

        // =====================================================================
        // USUARIOS
        // =====================================================================
        private static async Task<(Usuario cliente, List<Usuario> repartidores)> SeedUsuariosAsync(
            DeliveryDbContext context, ISeguridadService seguridadService)
        {
            var ahora = DateTime.UtcNow;

            var rolAdmin      = await context.Roles.FirstAsync(r => r.Nombre == "Admin");
            var rolCliente    = await context.Roles.FirstAsync(r => r.Nombre == "Cliente");
            var rolRepartidor = await context.Roles.FirstAsync(r => r.Nombre == "Repartidor");

            // Admin
            if (!await context.Usuarios.AnyAsync(u => u.Email == "admin@rayoexpres.com"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre       = "Administrador",
                    Apellidos    = "RayoExpres",
                    Email        = "admin@rayoexpres.com",
                    Telefono     = "0999999999",
                    PasswordHash = seguridadService.HashearPassword("Admin123*"),
                    RolId        = rolAdmin.Id,
                    TipoUsuario  = TipoUsuarioEnum.Administrador,
                    Activo       = true,
                    CreadoEn     = ahora
                });
            }

            // Cliente
            if (!await context.Usuarios.AnyAsync(u => u.Email == "cliente@rayoexpres.com"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre       = "Carlos",
                    Apellidos    = "Andrade Pozo",
                    Email        = "cliente@rayoexpres.com",
                    Telefono     = "0988123456",
                    PasswordHash = seguridadService.HashearPassword("Cliente123*"),
                    RolId        = rolCliente.Id,
                    TipoUsuario  = TipoUsuarioEnum.Cliente,
                    Activo       = true,
                    CreadoEn     = ahora
                });
            }

            // Repartidores (datos coherentes con nombres ecuatorianos)
            var repartidoresData = new[]
            {
                ("Juan",  "Revelo Torres",   "0978456123", "repartidor1@rayoexpres.com"),
                ("Mario", "Cuasquer Ibarra", "0962345678", "repartidor2@rayoexpres.com"),
                ("Luis",  "Narváez Pinto",   "0953214567", "repartidor3@rayoexpres.com"),
            };

            foreach (var (nombre, apellidos, telefono, email) in repartidoresData)
            {
                if (!await context.Usuarios.AnyAsync(u => u.Email == email))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre       = nombre,
                        Apellidos    = apellidos,
                        Email        = email,
                        Telefono     = telefono,
                        PasswordHash = seguridadService.HashearPassword("Repartidor123*"),
                        RolId        = rolRepartidor.Id,
                        TipoUsuario  = TipoUsuarioEnum.Repartidor,
                        Activo       = true,
                        CreadoEn     = ahora
                    });
                }
            }

            await context.SaveChangesAsync();

            var cliente      = await context.Usuarios.FirstAsync(u => u.Email == "cliente@rayoexpres.com");
            var listaRep     = await context.Usuarios
                .Where(u => u.TipoUsuario == TipoUsuarioEnum.Repartidor)
                .ToListAsync();

            return (cliente, listaRep);
        }

        // =====================================================================
        // RESTAURANTES + CATEGORÍAS + PRODUCTOS
        // =====================================================================
        private static async Task<List<Restaurante>> SeedRestaurantesYProductosAsync(DeliveryDbContext context, ISeguridadService seguridadService)
        {
            var ahora        = DateTime.UtcNow;
            var rolRestaurante = await context.Roles.FirstAsync(r => r.Nombre == "Restaurante");

            var restaurantesData = new[]
            {
                ("Parrilla Ibarra",       "Parrilladas",  "Los mejores cortes de carne asados a la parrilla en Ibarra.",   1.50m, "parrillaibarra@rayoexpres.com"),
                ("El Rincón Imbabureño",  "Comida Típica","Sabores auténticos de Imbabura: hornado, fritada y más.",        2.00m, "rinconimbabura@rayoexpres.com"),
                ("Pizza Andina",          "Pizza",        "Pizzas artesanales con insumos locales y toque andino.",         1.00m, "pizzaandina@rayoexpres.com"),
                ("Sabor Norteño",         "Mexicana",     "Tacos, burritos y quesadillas estilo norteño.",                  1.25m, "sabornorteno@rayoexpres.com"),
                ("Café Laguna",           "Cafetería",    "Café premium con vista a la laguna de Yahuarcocha.",             0.00m, "cafelaguna@rayoexpres.com"),
                ("Sushi Sakura Ibarra",   "Sushi",        "El mejor sushi de la Ciudad Blanca, rolls frescos cada día.",    2.50m, "sushisakura@rayoexpres.com"),
                ("Pollos del Valle",      "Pollo",        "Pollo broaster crujiente y jugoso, receta familiar.",            1.00m, "pollosdelvalle@rayoexpres.com"),
                ("Hamburguesas Volcán",   "Hamburguesas", "Hamburguesas dobles y gigantes estilo volcán.",                  1.50m, "hamburguesasvolcan@rayoexpres.com"),
                ("Tacos La Merced",       "Mexicana",     "Tacos al pastor y burritos preparados al momento.",              1.25m, "tacoslamerced@rayoexpres.com"),
                ("Pastas San Miguel",     "Italiana",     "Pastas caseras y lasagna al horno todos los días.",              1.75m, "pastassanmiguel@rayoexpres.com"),
                ("La Esquina del Hornado","Comida Típica","Hornado completo con llapingachos y mote.",                      1.50m, "esquinahornado@rayoexpres.com"),
                ("Green Bowl Ibarra",     "Ensaladas",    "Bowls nutritivos y opciones vegetarianas frescas.",              1.00m, "greenbowl@rayoexpres.com"),
            };

            foreach (var (nombre, categoria, desc, envio, email) in restaurantesData)
            {
                if (await context.Restaurantes.AnyAsync(r => r.Nombre == nombre))
                    continue;

                // Usuario propietario del restaurante
                if (!await context.Usuarios.AnyAsync(u => u.Email == email))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre       = nombre,
                        Apellidos    = "Administrador",
                        Email        = email,
                        Telefono     = "062600000",
                        PasswordHash = seguridadService.HashearPassword("Restaurante123*"),
                        RolId        = rolRestaurante.Id,
                        TipoUsuario  = TipoUsuarioEnum.Restaurante,
                        Activo       = true,
                        CreadoEn     = ahora
                    });
                    await context.SaveChangesAsync();
                }

                var usuarioPropietario = await context.Usuarios.FirstAsync(u => u.Email == email);

                var restaurante = new Restaurante
                {
                    Nombre         = nombre,
                    Descripcion    = desc,
                    Categoria      = categoria,
                    Calle          = "Av. Mariano Acosta",
                    Ciudad         = "Ibarra",
                    Telefono       = "062600000",
                    Email          = email,
                    Estado         = EstadoRestauranteEnum.Aprobado,
                    Abierto        = true,
                    LogoUrl        = null,
                    CostoEnvioBase = envio,
                    CreadoPor      = usuarioPropietario.Id,
                    CreadoEn       = ahora
                };
                context.Restaurantes.Add(restaurante);
                await context.SaveChangesAsync();

                await SeedCategoriasYProductosAsync(context, restaurante, categoria, ahora);
            }

            return await context.Restaurantes.ToListAsync();
        }

        private static async Task SeedCategoriasYProductosAsync(
            DeliveryDbContext context, Restaurante restaurante, string categoriaRestaurante, DateTime ahora)
        {
            // Crear una categoría principal por restaurante
            var categoria = new CategoriaProducto
            {
                RestauranteId = restaurante.Id,
                Nombre        = categoriaRestaurante,
                Descripcion   = $"Categoría principal de {restaurante.Nombre}",
                CreadoEn      = ahora
            };
            context.CategoriasProducto.Add(categoria);
            await context.SaveChangesAsync();

            // Productos específicos por tipo de restaurante
            var productosMap = new Dictionary<string, List<(string Nombre, decimal Precio)>>
            {
                ["Parrilladas"]  = new() { ("Churrasco completo", 12.50m), ("Costillas BBQ", 14.00m), ("Lomo fino a la plancha", 11.00m), ("Anticuchos de res", 8.50m), ("Parrilla mixta para 2", 22.00m), ("Chorizo ahumado", 6.00m), ("Morcilla asada", 5.50m), ("Chuleta de cerdo", 9.75m), ("Pinchos de pollo", 7.00m), ("Longaniza a la brasa", 6.50m) },
                ["Comida Típica"]= new() { ("Hornado completo", 9.00m), ("Fritada con mote", 8.50m), ("Seco de pollo", 7.50m), ("Caldo de patas", 5.00m), ("Llapingachos con chorizo", 6.50m), ("Yapingacho gratinado", 7.00m), ("Cazuela de mariscos", 10.00m), ("Caldo de gallina", 6.00m), ("Menudo de res", 5.50m), ("Fanesca (temporada)", 8.00m) },
                ["Pizza"]        = new() { ("Pizza Margarita personal", 6.50m), ("Pizza Hawaiana familiar", 15.00m), ("Pizza Cuatro Quesos", 13.50m), ("Pizza Pepperoni", 14.00m), ("Pizza Vegetariana", 12.00m), ("Calzone de jamón", 8.00m), ("Palitos de ajo", 3.50m), ("Cannelloni de carne", 9.00m), ("Lasagna bolognesa", 10.00m), ("Bruschetta", 4.50m) },
                ["Mexicana"]     = new() { ("Tacos al pastor x3", 7.50m), ("Burrito de pollo", 8.00m), ("Quesadilla de queso", 5.50m), ("Nachos con guacamole", 6.50m), ("Enchiladas verdes", 9.00m), ("Tostada con frijoles", 4.50m), ("Pozole rojo", 8.50m), ("Torta de chorizo", 7.00m), ("Sopa azteca", 6.00m), ("Tamales", 5.00m) },
                ["Cafetería"]    = new() { ("Cappuccino", 2.75m), ("Americano doble", 2.00m), ("Café latte", 3.00m), ("Moka helado", 3.50m), ("Croissant de mantequilla", 2.50m), ("Cheesecake de fresa", 4.00m), ("Brownie con helado", 3.75m), ("Tostada francesa", 3.25m), ("Granola bowl", 4.50m), ("Jugo natural de naranja", 2.25m) },
                ["Sushi"]        = new() { ("California Roll x8", 10.99m), ("Spicy Tuna Roll x8", 12.50m), ("Philadelphia Roll x8", 11.00m), ("Dragon Roll x8", 13.00m), ("Sashimi salmón x6", 14.00m), ("Nigiri atún x4", 9.50m), ("Maki vegetariano x8", 9.00m), ("Tempura ebi x6", 11.50m), ("Gyoza al vapor x6", 7.50m), ("Edamame salado", 4.00m) },
                ["Pollo"]        = new() { ("Pollo broaster 1/4", 6.99m), ("Pollo broaster 1/2", 11.50m), ("Alitas BBQ x8", 8.75m), ("Pechuga a la plancha", 7.50m), ("Nuggets x10", 5.50m), ("Combo familiar broaster", 24.00m), ("Ensalada de pollo", 6.00m), ("Sopa de pollo con fideos", 4.50m), ("Pincho de pollo x3", 7.00m), ("Pollo teriyaki", 9.00m) },
                ["Hamburguesas"] = new() { ("Hamburguesa clásica", 5.99m), ("Hamburguesa doble carne", 7.99m), ("Hamburguesa volcán BBQ", 9.50m), ("Hamburguesa vegetariana", 6.50m), ("Combo familiar x4", 28.00m), ("Papas fritas medianas", 2.50m), ("Papas fritas grandes", 3.50m), ("Onion rings", 3.00m), ("Milkshake vainilla", 4.00m), ("Hot dog gourmet", 5.00m) },
                ["Italiana"]     = new() { ("Spaghetti bolognesa", 8.50m), ("Fettuccine alfredo", 9.00m), ("Lasagna de carne", 10.50m), ("Ravioli de espinaca", 9.50m), ("Risotto parmesano", 11.00m), ("Gnocchi casero", 8.00m), ("Minestrone", 5.50m), ("Tiramisu", 4.50m), ("Panna cotta", 4.00m), ("Pan ciabatta con aceite", 2.50m) },
                ["Ensaladas"]    = new() { ("Ensalada César", 5.50m), ("Bowl de quinoa y vegetales", 7.00m), ("Ensalada griega", 6.50m), ("Bowl proteico de atún", 8.00m), ("Ensalada caprese", 6.00m), ("Wrap vegetariano", 5.00m), ("Bowl de frutas tropicales", 4.50m), ("Smoothie verde detox", 3.75m), ("Hummus con pita", 4.00m), ("Tabbouleh de bulgur", 5.50m) },
            };

            // Fallback si la categoría no está en el mapa
            if (!productosMap.TryGetValue(categoriaRestaurante, out var productos))
            {
                productos = new() { ("Plato del día", 7.00m), ("Sopa del día", 4.00m), ("Bebida natural", 2.00m) };
            }

            foreach (var (nombreProd, precio) in productos)
            {
                context.Productos.Add(new Producto
                {
                    RestauranteId     = restaurante.Id,
                    CategoriaId       = categoria.Id,
                    Nombre            = nombreProd,
                    Descripcion       = $"{nombreProd} preparado con ingredientes frescos de la región.",
                    Precio            = precio,
                    Disponible        = true,
                    TiempoPreparacion = new Random().Next(10, 30),
                    CreadoEn          = ahora
                });
            }
            await context.SaveChangesAsync();
        }

        // =====================================================================
        // CUPONES
        // =====================================================================
        private static async Task SeedCuponesAsync(DeliveryDbContext context)
        {
            if (await context.Cupones.AnyAsync()) return;

            var ahora = DateTime.UtcNow;
            context.Cupones.AddRange(
                new Cupon
                {
                    Codigo         = "BIENVENIDO10",
                    TipoDescuento  = TipoDescuentoEnum.Porcentaje,
                    ValorDescuento = 10m,
                    FechaInicio    = ahora,
                    FechaFin       = ahora.AddMonths(3),
                    Activo         = true
                },
                new Cupon
                {
                    Codigo         = "ENVIOGRATIS",
                    TipoDescuento  = TipoDescuentoEnum.MontoFijo,
                    ValorDescuento = 2.50m,
                    FechaInicio    = ahora,
                    FechaFin       = ahora.AddMonths(1),
                    Activo         = true
                },
                new Cupon
                {
                    Codigo         = "RAYO20",
                    TipoDescuento  = TipoDescuentoEnum.Porcentaje,
                    ValorDescuento = 20m,
                    PedidoMinimo   = 25.00m,
                    FechaInicio    = ahora,
                    FechaFin       = ahora.AddMonths(6),
                    Activo         = true
                }
            );
            await context.SaveChangesAsync();
        }

        // =====================================================================
        // REPARTIDORES + VEHÍCULOS
        // =====================================================================
        private static async Task SeedRepartidoresYVehiculosAsync(
            DeliveryDbContext context, List<Usuario> usuariosRepartidores)
        {
            var ahora = DateTime.UtcNow;

            // Datos realistas: Licencia | Placa | Marca | Modelo | Color
            var datosRepartidores = new[]
            {
                ("LIC-IMB-4521987", "PBX-1234", "Honda",   "Wave110",  "Rojo",   TipoVehiculoEnum.Motocicleta),
                ("LIC-IMB-7823654", "PBX-5678", "Yamaha",  "YBR125",   "Negro",  TipoVehiculoEnum.Motocicleta),
                ("LIC-IMB-1093482", "PBX-9012", "Suzuki",  "GN125",    "Azul",   TipoVehiculoEnum.Motocicleta),
            };

            for (int i = 0; i < usuariosRepartidores.Count; i++)
            {
                var usuarioId = usuariosRepartidores[i].Id;

                if (await context.Repartidores.AnyAsync(r => r.UsuarioId == usuarioId))
                    continue;

                var (licencia, placa, marca, modelo, color, tipoVehiculo) = datosRepartidores[i % datosRepartidores.Length];

                // Repartidor — campos NOT NULL: UsuarioId, LicenciaConducir, CreadoEn
                // EstadoAprobacion es enum con valor por defecto 0
                var repartidor = new Repartidor
                {
                    UsuarioId           = usuarioId,
                    LicenciaConducir    = licencia,
                    EstadoAprobacion    = EstadoAprobacionEnum.Aprobado,
                    Disponible          = true,
                    CalificacionPromedio = 4.8m,
                    CreadoEn            = ahora
                };
                context.Repartidores.Add(repartidor);
                await context.SaveChangesAsync();

                // Vehículo — campos NOT NULL: Placa, Marca, Modelo, CreadoEn
                context.Vehiculos.Add(new Vehiculo
                {
                    RepartidorId = repartidor.UsuarioId,
                    TipoVehiculo = tipoVehiculo,
                    Placa        = placa,
                    Marca        = marca,
                    Modelo       = modelo,
                    Anio         = 2022,
                    Color        = color,
                    CreadoEn     = ahora
                });
                await context.SaveChangesAsync();
            }
        }

        // =====================================================================
        // DIRECCIÓN + PEDIDOS + DETALLES
        // =====================================================================
        private static async Task SeedDireccionYPedidosAsync(
            DeliveryDbContext context,
            Usuario cliente,
            List<Restaurante> restaurantes,
            List<Usuario> usuariosRepartidores)
        {
            if (await context.Pedidos.AnyAsync()) return;

            var ahora = DateTime.UtcNow;

            // Dirección del cliente — campos NOT NULL: Calle, Ciudad, CreadoEn
            var direccion = await context.Direcciones.FirstOrDefaultAsync(d => d.UsuarioId == cliente.Id);
            if (direccion == null)
            {
                direccion = new Direccion
                {
                    UsuarioId    = cliente.Id,
                    Alias        = "Casa",
                    Calle        = "Av. El Retorno",
                    Numero       = "14-82",
                    Ciudad       = "Ibarra",
                    Referencia   = "Junto al parque La Familia",
                    EsPrincipal  = true,
                    CreadoEn     = ahora
                };
                context.Direcciones.Add(direccion);
                await context.SaveChangesAsync();
            }

            var repartidores = await context.Repartidores.ToListAsync();
            var rng          = new Random(42); // Semilla fija para reproducibilidad
            var estados      = new[]
            {
                EstadoPedidoEnum.Entregado,
                EstadoPedidoEnum.Entregado,
                EstadoPedidoEnum.Entregado,
                EstadoPedidoEnum.Cancelado,
                EstadoPedidoEnum.Entregado,
            };

            for (int i = 0; i < 5; i++)
            {
                var restaurante = restaurantes[i % restaurantes.Count];
                var repartidor  = repartidores.Count > 0 ? repartidores[i % repartidores.Count] : null;

                var productos = await context.Productos
                    .Where(p => p.RestauranteId == restaurante.Id && p.Disponible)
                    .Take(2)
                    .ToListAsync();

                decimal subtotal = productos.Sum(p => p.Precio);
                decimal envio    = restaurante.CostoEnvioBase;

                var pedido = new Pedido
                {
                    UsuarioId         = cliente.Id,
                    RestauranteId     = restaurante.Id,
                    RepartidorId      = repartidor?.UsuarioId,
                    DireccionEntregaId = direccion.Id,
                    Subtotal          = subtotal > 0 ? subtotal : 10.00m,
                    CostoEnvio        = envio,
                    Total             = (subtotal > 0 ? subtotal : 10.00m) + envio,
                    EstadoPedido      = estados[i],
                    TipoMetodoPago    = TipoMetodoPagoEnum.Efectivo,
                    FechaPedido       = ahora.AddDays(-(i + 1) * 3)
                };
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();

                // Detalles de pedido — todos los campos son NOT NULL por lógica
                foreach (var prod in productos)
                {
                    context.DetallesPedido.Add(new DetallePedido
                    {
                        PedidoId       = pedido.Id,
                        ProductoId     = prod.Id,
                        Cantidad       = 1,
                        PrecioUnitario = prod.Precio,
                        Subtotal       = prod.Precio
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
