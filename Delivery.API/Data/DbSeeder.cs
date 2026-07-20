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

            await SeedRolesAsync(context);
            var (cliente, repartidores) = await SeedUsuariosAsync(context, seguridadService);
            var restaurantes = await SeedRestaurantesYProductosAsync(context, seguridadService);
            await SeedCuponesAsync(context);
            await SeedRepartidoresYVehiculosAsync(context, repartidores);
            await SeedDireccionYPedidosAsync(context, cliente, restaurantes, repartidores);

            // Parche: Asignar imágenes a productos existentes que no tengan
            await PatchProductosSinImagenAsync(context);

            // Parche: Asignar coordenadas a restaurantes que no tengan
            await PatchRestaurantesSinCoordenadasAsync(context);

            // Parche: Establecer como Disponible a repartidores ya aprobados que estén Desconectados
            await PatchRepartidoresAprobadosAsync(context);

            // Parche Liquid Glass: Actualizar imágenes de restaurantes existentes y variar sus coordenadas en Ibarra
            await PatchRestaurantesLiquidGlassImagesAsync(context);

            // Parche: Confirmar correos específicos
            await PatchEmailsVerificadosAsync(context);

            // Parche: Nuevas fotos específicas
            await PatchProductosEspecificosImagesAsync(context);
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
                    Cedula       = "1000000000",
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
                    Cedula       = "1000000001",
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
                ("Juan",  "Revelo Torres",   "0978456123", "repartidor1@rayoexpres.com", "1000000002"),
                ("Mario", "Cuasquer Ibarra", "0962345678", "repartidor2@rayoexpres.com", "1000000003"),
                ("Luis",  "Narváez Pinto",   "0953214567", "repartidor3@rayoexpres.com", "1000000004"),
            };

            foreach (var (nombre, apellidos, telefono, email, cedula) in repartidoresData)
            {
                if (!await context.Usuarios.AnyAsync(u => u.Email == email))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre       = nombre,
                        Apellidos    = apellidos,
                        Email        = email,
                        Telefono     = telefono,
                        Cedula       = cedula,
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
                // NUEVOS RESTAURANTES (Liquid Glass)
                ("Wok & Roll",            "Asiática",     "Fideos salteados, arroz frito y comida oriental urbana.",        1.75m, "wokandroll@rayoexpres.com"),
                ("Sweet Dreams Bakery",   "Postres",      "Pasteles, cheesecakes y macarons artesanales.",                  1.00m, "sweetdreams@rayoexpres.com"),
                ("Desayunos de la Abuela","Desayunos",    "Desayunos completos, bolones, tigrillos y café pasado.",         1.25m, "desayunosabuela@rayoexpres.com"),
                ("El Rey del Marisco",    "Mariscos",     "Ceviches, encocados y parrilladas de mariscos.",                 2.00m, "reymarisco@rayoexpres.com"),
                ("Shawarma Express",      "Árabe",        "Shawarmas, falafel y comida árabe rápida.",                      1.25m, "shawarmaexpress@rayoexpres.com"),
                ("Vegan Paradise",        "Vegetariana",  "Opciones 100% plant-based, hamburguesas de lenteja.",            1.50m, "veganparadise@rayoexpres.com")
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
                        Cedula       = "100" + new Random().Next(1000000, 9999999).ToString(), // Fake unique cedula
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
            var productosMap = new Dictionary<string, List<(string Nombre, decimal Precio, string Imagen)>>
            {
                ["Parrilladas"]  = new() { ("Churrasco completo", 12.50m, "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=500&q=80"), ("Costillas BBQ", 14.00m, "https://images.unsplash.com/photo-1544025162-811114215449?w=500&q=80"), ("Lomo fino a la plancha", 11.00m, "https://images.unsplash.com/photo-1572449043416-55f4685c9bb7?w=500&q=80"), ("Anticuchos de res", 8.50m, "https://images.unsplash.com/photo-1599598425947-33002629b30b?w=500&q=80") },
                ["Comida Típica"]= new() { ("Hornado completo", 9.00m, "https://images.unsplash.com/photo-1582169505937-b9992bd01ed9?w=500&q=80"), ("Fritada con mote", 8.50m, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80"), ("Seco de pollo", 7.50m, "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=500&q=80"), ("Caldo de patas", 5.00m, "https://images.unsplash.com/photo-1547592180-85f173990554?w=500&q=80") },
                ["Pizza"]        = new() { ("Pizza Margarita", 6.50m, "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=500&q=80"), ("Pizza Hawaiana", 15.00m, "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500&q=80"), ("Pizza Cuatro Quesos", 13.50m, "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=500&q=80"), ("Pizza Pepperoni", 14.00m, "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=500&q=80") },
                ["Mexicana"]     = new() { ("Tacos al pastor x3", 7.50m, "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?w=500&q=80"), ("Burrito de pollo", 8.00m, "https://images.unsplash.com/photo-1626700051175-6818013e1d4f?w=500&q=80"), ("Quesadilla", 5.50m, "https://images.unsplash.com/photo-1618040996328-b9894e24eb29?w=500&q=80"), ("Nachos", 6.50m, "https://images.unsplash.com/photo-1513456852971-30c0b8199d4d?w=500&q=80") },
                ["Cafetería"]    = new() { ("Cappuccino", 2.75m, "https://images.unsplash.com/photo-1512568400610-62da28bc8a13?w=500&q=80"), ("Americano", 2.00m, "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=500&q=80"), ("Croissant", 2.50m, "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=500&q=80"), ("Cheesecake", 4.00m, "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?w=500&q=80") },
                ["Sushi"]        = new() { ("California Roll", 10.99m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=500&q=80"), ("Spicy Tuna", 12.50m, "https://images.unsplash.com/photo-1553621042-f6e147245754?w=500&q=80"), ("Sashimi", 14.00m, "https://images.unsplash.com/photo-1534482421-64566f976cfa?w=500&q=80"), ("Nigiri", 9.50m, "https://images.unsplash.com/photo-1611143669185-af224c5e3252?w=500&q=80") },
                ["Pollo"]        = new() { ("Pollo broaster", 6.99m, "https://images.unsplash.com/photo-1562967914-608f82629710?w=500&q=80"), ("Alitas BBQ", 8.75m, "https://images.unsplash.com/photo-1608039829572-78524f79c4c7?w=500&q=80"), ("Nuggets", 5.50m, "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=500&q=80"), ("Pechuga", 7.50m, "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=500&q=80") },
                ["Hamburguesas"] = new() { ("Hamburguesa clásica", 5.99m, "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=500&q=80"), ("Doble carne", 7.99m, "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=500&q=80"), ("Volcán BBQ", 9.50m, "https://images.unsplash.com/photo-1550547660-d9450f859349?w=500&q=80"), ("Vegetariana", 6.50m, "https://images.unsplash.com/photo-1520072959219-c595dc870360?w=500&q=80") },
                ["Italiana"]     = new() { ("Spaghetti bolognesa", 8.50m, "https://images.unsplash.com/photo-1621996311210-91136b856e87?w=500&q=80"), ("Fettuccine alfredo", 9.00m, "https://images.unsplash.com/photo-1645112411341-6c4fd023714a?w=500&q=80"), ("Lasagna", 10.50m, "https://images.unsplash.com/photo-1619895092538-128341789043?w=500&q=80"), ("Ravioli", 9.50m, "https://images.unsplash.com/photo-1551183053-bf91a1d81141?w=500&q=80") },
                ["Ensaladas"]    = new() { ("Ensalada César", 5.50m, "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=500&q=80"), ("Bowl de quinoa", 7.00m, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80"), ("Ensalada griega", 6.50m, "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=500&q=80"), ("Bowl proteico", 8.00m, "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=500&q=80") },
                ["Asiática"]     = new() { ("Chow Mein", 6.50m, "https://images.unsplash.com/photo-1585032226651-759b368d7246?w=500&q=80"), ("Arroz frito", 5.00m, "https://images.unsplash.com/photo-1603133872878-684f208fb84b?w=500&q=80"), ("Rollitos de primavera", 3.50m, "https://images.unsplash.com/photo-1544378730-8b5a0cb08197?w=500&q=80") },
                ["Postres"]      = new() { ("Cheesecake de frutos rojos", 4.50m, "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?w=500&q=80"), ("Tarta de chocolate", 3.50m, "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=500&q=80"), ("Macarons x6", 6.00m, "https://images.unsplash.com/photo-1569864358642-9d1684040f43?w=500&q=80") },
                ["Desayunos"]    = new() { ("Bolón mixto", 3.50m, "https://images.unsplash.com/photo-1628191010210-a59de33e5941?w=500&q=80"), ("Tigrillo", 4.50m, "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=500&q=80"), ("Desayuno continental", 5.00m, "https://images.unsplash.com/photo-1533089860892-a7c6f0a88666?w=500&q=80") },
                ["Mariscos"]     = new() { ("Ceviche de camarón", 8.00m, "https://images.unsplash.com/photo-1599084990807-33458368615c?w=500&q=80"), ("Encocado de pescado", 9.50m, "https://images.unsplash.com/photo-1615719413546-198b25453f85?w=500&q=80"), ("Arroz marinero", 10.00m, "https://images.unsplash.com/photo-1534080564583-6be75777b70a?w=500&q=80") },
                ["Árabe"]        = new() { ("Shawarma mixto", 5.50m, "https://images.unsplash.com/photo-1528735602780-2552fd46c7af?w=500&q=80"), ("Falafel", 4.00m, "https://images.unsplash.com/photo-1593006526979-4f8fb252669e?w=500&q=80"), ("Hummus con pan pita", 3.50m, "https://images.unsplash.com/photo-1576082260775-690a424231b2?w=500&q=80") },
                ["Vegetariana"]  = new() { ("Burger de lenteja", 6.50m, "https://images.unsplash.com/photo-1520072959219-c595dc870360?w=500&q=80"), ("Wrap de vegetales", 5.00m, "https://images.unsplash.com/photo-1626700051175-6818013e1d4f?w=500&q=80"), ("Bowl vegano", 7.50m, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80") }
            };

            // Fallback si la categoría no está en el mapa
            if (!productosMap.TryGetValue(categoriaRestaurante, out var productos))
            {
                productos = new() { ("Plato del día", 7.00m, "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500&q=80"), ("Sopa del día", 4.00m, "https://images.unsplash.com/photo-1547592180-85f173990554?w=500&q=80"), ("Bebida natural", 2.00m, "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=500&q=80") };
            }

            foreach (var (nombreProd, precio, imagen) in productos)
            {
                context.Productos.Add(new Producto
                {
                    RestauranteId     = restaurante.Id,
                    CategoriaId       = categoria.Id,
                    Nombre            = nombreProd,
                    Descripcion       = $"{nombreProd} preparado con ingredientes frescos de la región.",
                    Precio            = precio,
                    ImagenUrl         = imagen,
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
                    Estado              = Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible,
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

        // =====================================================================
        // PARCHE: IMÁGENES EN PRODUCTOS EXISTENTES
        // =====================================================================
        private static async Task PatchProductosSinImagenAsync(DeliveryDbContext context)
        {
            var productosSinImagen = await context.Productos
                .Include(p => p.Categoria)
                .Where(p => string.IsNullOrEmpty(p.ImagenUrl))
                .ToListAsync();

            if (!productosSinImagen.Any()) return;

            var imagenesPorCategoria = new Dictionary<string, List<string>>
            {
                ["Parrilladas"]  = new() { "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=500&q=80", "https://images.unsplash.com/photo-1544025162-811114215449?w=500&q=80", "https://images.unsplash.com/photo-1572449043416-55f4685c9bb7?w=500&q=80", "https://images.unsplash.com/photo-1599598425947-33002629b30b?w=500&q=80" },
                ["Comida Típica"]= new() { "https://images.unsplash.com/photo-1582169505937-b9992bd01ed9?w=500&q=80", "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80", "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=500&q=80", "https://images.unsplash.com/photo-1547592180-85f173990554?w=500&q=80" },
                ["Pizza"]        = new() { "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=500&q=80", "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500&q=80", "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=500&q=80", "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=500&q=80" },
                ["Mexicana"]     = new() { "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?w=500&q=80", "https://images.unsplash.com/photo-1626700051175-6818013e1d4f?w=500&q=80", "https://images.unsplash.com/photo-1618040996328-b9894e24eb29?w=500&q=80", "https://images.unsplash.com/photo-1513456852971-30c0b8199d4d?w=500&q=80" },
                ["Cafetería"]    = new() { "https://images.unsplash.com/photo-1512568400610-62da28bc8a13?w=500&q=80", "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=500&q=80", "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=500&q=80", "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?w=500&q=80" },
                ["Sushi"]        = new() { "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=500&q=80", "https://images.unsplash.com/photo-1553621042-f6e147245754?w=500&q=80", "https://images.unsplash.com/photo-1534482421-64566f976cfa?w=500&q=80", "https://images.unsplash.com/photo-1611143669185-af224c5e3252?w=500&q=80" },
                ["Pollo"]        = new() { "https://images.unsplash.com/photo-1562967914-608f82629710?w=500&q=80", "https://images.unsplash.com/photo-1608039829572-78524f79c4c7?w=500&q=80", "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=500&q=80", "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=500&q=80" },
                ["Hamburguesas"] = new() { "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=500&q=80", "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=500&q=80", "https://images.unsplash.com/photo-1550547660-d9450f859349?w=500&q=80", "https://images.unsplash.com/photo-1520072959219-c595dc870360?w=500&q=80" },
                ["Italiana"]     = new() { "https://images.unsplash.com/photo-1621996311210-91136b856e87?w=500&q=80", "https://images.unsplash.com/photo-1645112411341-6c4fd023714a?w=500&q=80", "https://images.unsplash.com/photo-1619895092538-128341789043?w=500&q=80", "https://images.unsplash.com/photo-1551183053-bf91a1d81141?w=500&q=80" },
                ["Ensaladas"]    = new() { "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=500&q=80", "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80", "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=500&q=80", "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=500&q=80" },
            };
            
            var genericImages = new List<string> { "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500&q=80", "https://images.unsplash.com/photo-1547592180-85f173990554?w=500&q=80", "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=500&q=80" };
            
            var rng = new Random();

            foreach (var prod in productosSinImagen)
            {
                var categoriaNombre = prod.Categoria?.Nombre ?? "";
                if (imagenesPorCategoria.TryGetValue(categoriaNombre, out var imagenes))
                {
                    prod.ImagenUrl = imagenes[rng.Next(imagenes.Count)];
                }
                else
                {
                    prod.ImagenUrl = genericImages[rng.Next(genericImages.Count)];
                }
            }

            await context.SaveChangesAsync();
        }

        // =====================================================================
        // PARCHE: COORDENADAS DE RESTAURANTES EXISTENTES
        // =====================================================================
        private static async Task PatchRestaurantesSinCoordenadasAsync(DeliveryDbContext context)
        {
            var restaurantesSinCoords = await context.Restaurantes
                .Where(r => r.Latitud == null || r.Longitud == null)
                .ToListAsync();

            if (!restaurantesSinCoords.Any()) return;

            var rng = new Random();
            // Centro de Ibarra aproximado
            double baseLat = 0.3517;
            double baseLng = -78.1222;

            foreach (var rest in restaurantesSinCoords)
            {
                // Variación pequeña de +- 0.02 para distribuirlos en la ciudad
                double latOffset = (rng.NextDouble() - 0.5) * 0.04;
                double lngOffset = (rng.NextDouble() - 0.5) * 0.04;
                
                rest.Latitud = (decimal)(baseLat + latOffset);
                rest.Longitud = (decimal)(baseLng + lngOffset);
            }

            await context.SaveChangesAsync();
        }

        // =====================================================================
        // PARCHE: REPARTIDORES APROBADOS
        // =====================================================================
        private static async Task PatchRepartidoresAprobadosAsync(DeliveryDbContext context)
        {
            var repartidoresBuggeados = await context.Repartidores
                .Where(r => r.EstadoAprobacion == Delivery.Modelos.Enums.EstadoAprobacionEnum.Aprobado && 
                            r.Estado == Delivery.Modelos.Enums.EstadoRepartidorEnum.Desconectado)
                .ToListAsync();

            if (!repartidoresBuggeados.Any()) return;

            foreach (var rep in repartidoresBuggeados)
            {
                rep.Estado = Delivery.Modelos.Enums.EstadoRepartidorEnum.Disponible;
            }

            await context.SaveChangesAsync();
        }

        // =====================================================================
        // PARCHE: LIQUID GLASS IMAGES & LOCATIONS
        // =====================================================================
        private static async Task PatchRestaurantesLiquidGlassImagesAsync(DeliveryDbContext context)
        {
            var restaurantes = await context.Restaurantes.ToListAsync();
            if (!restaurantes.Any()) return;

            var rng = new Random();
            double baseLat = 0.3517;
            double baseLng = -78.1222;

            foreach (var rest in restaurantes)
            {
                // Variar coordenadas geográficas fuertemente para dispersar restaurantes en el mapa
                double latOffset = (rng.NextDouble() - 0.5) * 0.08;
                double lngOffset = (rng.NextDouble() - 0.5) * 0.08;
                rest.Latitud = (decimal)(baseLat + latOffset);
                rest.Longitud = (decimal)(baseLng + lngOffset);

                // Asignar los logos estéticos generados
                if (rest.Categoria == "Hamburguesas" || rest.Categoria == "Pollo")
                    rest.LogoUrl = "/img/restaurantes/burger.png";
                else if (rest.Categoria == "Pizza" || rest.Categoria == "Italiana")
                    rest.LogoUrl = "/img/restaurantes/pizza.png";
                else if (rest.Categoria == "Sushi" || rest.Categoria == "Asiática")
                    rest.LogoUrl = "/img/restaurantes/sushi.png";
                else if (string.IsNullOrEmpty(rest.LogoUrl))
                {
                    // Random aesthetic images for others
                    var genericAesthetics = new[] {
                        "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=600&q=80",
                        "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=600&q=80",
                        "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=600&q=80",
                        "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?w=600&q=80"
                    };
                    rest.LogoUrl = genericAesthetics[rng.Next(genericAesthetics.Length)];
                }
            }

            await context.SaveChangesAsync();
        }

        // =====================================================================
        // PARCHE: CONFIRMAR CORREOS
        // =====================================================================
        private static async Task PatchEmailsVerificadosAsync(DeliveryDbContext context)
        {
            var emailsAConfirmar = new[] { "admin@rayoexpres.com", "davidtomas@gmail.com", "admin@admin.com" };
            var usuarios = await context.Usuarios
                .Where(u => emailsAConfirmar.Contains(u.Email))
                .ToListAsync();

            if (!usuarios.Any()) return;

            foreach (var user in usuarios)
            {
                user.EmailConfirmado = true;
            }

            await context.SaveChangesAsync();
        }

        // =====================================================================
        // PARCHE: FOTOS ESPECIFICAS PARA ALGUNOS PRODUCTOS
        // =====================================================================
        private static async Task PatchProductosEspecificosImagesAsync(DeliveryDbContext context)
        {
            var updateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Costillas BBQ", "/img/productos/costillas_bbq.png" },
                { "Anticuchos de res", "/img/productos/anticuchos_res.png" },
                { "Quesadilla", "/img/productos/quesadilla.png" },
                { "Spaghetti bolognesa", "/img/productos/spaghetti_bolognese.png" }
            };

            var productos = await context.Productos
                .Where(p => updateMap.Keys.Contains(p.Nombre))
                .ToListAsync();

            foreach(var prod in productos)
            {
                if(updateMap.TryGetValue(prod.Nombre, out string newImg))
                {
                    prod.ImagenUrl = newImg;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
