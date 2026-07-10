using Microsoft.EntityFrameworkCore;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.Enums;

namespace Delivery.Modelos
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Direccion> Direcciones { get; set; } = null!;
        public DbSet<Restaurante> Restaurantes { get; set; } = null!;
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;

        public DbSet<Repartidor> Repartidores { get; set; } = null!;
        public DbSet<Vehiculo> Vehiculos { get; set; } = null!;
        public DbSet<UbicacionActualRepartidor> UbicacionesActuales { get; set; } = null!;
        public DbSet<HistorialAsignacionesRepartidor> HistorialAsignaciones { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<DetallePedido> DetallesPedido { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<Resena> Resenas { get; set; } = null!;
        public DbSet<Cupon> Cupones { get; set; } = null!;
        public DbSet<CuponUsuario> CuponesUsuarios { get; set; } = null!;
        public DbSet<Favorito> Favoritos { get; set; } = null!;
        public DbSet<RegistroAuditoria> RegistrosAuditoria { get; set; } = null!;
        public DbSet<Notificacion> Notificaciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // NOTA: Las columnas de estado/tipo en la BD son INTEGER.
            // Se omite HasPostgresEnum para que EF Core convierta int <-> enum correctamente.
            // Si se usa HasPostgresEnum, Npgsql espera columnas de tipo texto nativo, lo que
            // produce fallos silenciosos al filtrar con LINQ.

            // Configuración Fluent API
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Direccion>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurante>()
                .HasOne(r => r.UsuarioAprobador)
                .WithMany()
                .HasForeignKey(r => r.AprobadoPor)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Restaurante>()
                .HasOne(r => r.UsuarioCreador)
                .WithMany()
                .HasForeignKey(r => r.CreadoPor)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CategoriaProducto>()
                .HasOne(c => c.Restaurante)
                .WithMany(r => r.Categorias)
                .HasForeignKey(c => c.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Restaurante)
                .WithMany(r => r.Productos)
                .HasForeignKey(p => p.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Repartidor>()
                .HasOne(r => r.Usuario)
                .WithOne()
                .HasForeignKey<Repartidor>(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Repartidor>()
                .HasOne(r => r.UsuarioAprobador)
                .WithMany()
                .HasForeignKey(r => r.AprobadoPor)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Repartidor)
                .WithMany(r => r.Vehiculos)
                .HasForeignKey(v => v.RepartidorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UbicacionActualRepartidor>()
                .HasOne(u => u.Repartidor)
                .WithOne(r => r.UbicacionActual)
                .HasForeignKey<UbicacionActualRepartidor>(u => u.RepartidorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialAsignacionesRepartidor>()
                .HasOne(h => h.Repartidor)
                .WithMany(r => r.HistorialAsignaciones)
                .HasForeignKey(h => h.RepartidorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialAsignacionesRepartidor>()
                .HasOne(h => h.Pedido)
                .WithMany()
                .HasForeignKey(h => h.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Restaurante)
                .WithMany()
                .HasForeignKey(p => p.RestauranteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Repartidor)
                .WithMany()
                .HasForeignKey(p => p.RepartidorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.DireccionEntrega)
                .WithMany()
                .HasForeignKey(p => p.DireccionEntregaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Pedido)
                .WithMany(p => p.Pagos)
                .HasForeignKey(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Resenas
            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Pedido)
                .WithMany()
                .HasForeignKey(r => r.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Restaurante)
                .WithMany()
                .HasForeignKey(r => r.RestauranteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Repartidor)
                .WithMany()
                .HasForeignKey(r => r.RepartidorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cupones Usuarios (Composite Key)
            modelBuilder.Entity<CuponUsuario>()
                .HasKey(cu => new { cu.CuponId, cu.UsuarioId, cu.PedidoId });

            modelBuilder.Entity<CuponUsuario>()
                .HasOne(cu => cu.Cupon)
                .WithMany(c => c.CuponesUsuarios)
                .HasForeignKey(cu => cu.CuponId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CuponUsuario>()
                .HasOne(cu => cu.Usuario)
                .WithMany()
                .HasForeignKey(cu => cu.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CuponUsuario>()
                .HasOne(cu => cu.Pedido)
                .WithMany()
                .HasForeignKey(cu => cu.PedidoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cupon
            modelBuilder.Entity<Cupon>()
                .HasOne(c => c.Restaurante)
                .WithMany()
                .HasForeignKey(c => c.RestauranteId)
                .OnDelete(DeleteBehavior.SetNull);

            // Favoritos (Composite Key)
            modelBuilder.Entity<Favorito>()
                .HasKey(f => new { f.UsuarioId, f.RestauranteId });

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Usuario)
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Restaurante)
                .WithMany()
                .HasForeignKey(f => f.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Registro Auditoria
            modelBuilder.Entity<RegistroAuditoria>()
                .HasOne(ra => ra.Usuario)
                .WithMany()
                .HasForeignKey(ra => ra.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Notificaciones
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

