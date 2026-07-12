using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class RestauranteService : IRestauranteService
    {
        private readonly DeliveryDbContext _context;

        public RestauranteService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurante>> GetAllAsync()
        {
            return await _context.Restaurantes
                .Include(r => r.UsuarioCreador)
                .Include(r => r.UsuarioAprobador)
                .ToListAsync();
        }

        public async Task<Restaurante?> GetByIdAsync(long id)
        {
            return await _context.Restaurantes
                .Include(r => r.UsuarioCreador)
                .Include(r => r.UsuarioAprobador)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Restaurante> CreateAsync(Restaurante restaurante)
        {
            restaurante.CreadoEn = System.DateTime.UtcNow;
            _context.Restaurantes.Add(restaurante);
            await _context.SaveChangesAsync();
            return restaurante;
        }

        public async Task<Restaurante> UpdateAsync(Restaurante restaurante)
        {
            var existing = await _context.Restaurantes.FindAsync(restaurante.Id);
            if (existing == null) return null!;

            existing.Nombre = restaurante.Nombre;
            existing.Descripcion = restaurante.Descripcion;
            existing.Categoria = restaurante.Categoria;
            existing.Calle = restaurante.Calle;
            existing.Ciudad = restaurante.Ciudad;
            existing.Latitud = restaurante.Latitud;
            existing.Longitud = restaurante.Longitud;
            existing.Telefono = restaurante.Telefono;
            existing.Email = restaurante.Email;
            existing.LogoUrl = restaurante.LogoUrl;
            existing.PortadaUrl = restaurante.PortadaUrl;
            existing.RedesSociales = restaurante.RedesSociales;
            existing.HoraApertura = restaurante.HoraApertura;
            existing.HoraCierre = restaurante.HoraCierre;
            existing.CostoEnvioBase = restaurante.CostoEnvioBase;
            existing.Abierto = restaurante.Abierto;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null) return false;

            var tienePedidos = await _context.Pedidos.AnyAsync(p => p.RestauranteId == id);
            if (tienePedidos)
            {
                restaurante.Abierto = false;
                restaurante.Estado = Delivery.Modelos.Enums.EstadoRestauranteEnum.Suspendido;
                restaurante.ActualizadoEn = System.DateTime.UtcNow;
                _context.Restaurantes.Update(restaurante);
                await _context.SaveChangesAsync();
                return true;
            }

            _context.Restaurantes.Remove(restaurante);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
