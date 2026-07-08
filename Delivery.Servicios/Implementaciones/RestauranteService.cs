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
            return await _context.Restaurantes.ToListAsync();
        }

        public async Task<Restaurante?> GetByIdAsync(long id)
        {
            return await _context.Restaurantes.FindAsync(id);
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
            existing.HoraApertura = restaurante.HoraApertura;
            existing.HoraCierre = restaurante.HoraCierre;
            existing.CostoEnvioBase = restaurante.CostoEnvioBase;
            existing.Estado = restaurante.Estado;
            existing.AprobadoPor = restaurante.AprobadoPor;
            existing.FechaAprobacion = restaurante.FechaAprobacion;
            existing.Abierto = restaurante.Abierto;
            existing.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null) return false;

            _context.Restaurantes.Remove(restaurante);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
