using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class CuponService : ICuponService
    {
        private readonly DeliveryDbContext _context;

        public CuponService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cupon>> GetAllAsync()
        {
            return await _context.Cupones.ToListAsync();
        }

        public async Task<Cupon?> GetByIdAsync(long id)
        {
            return await _context.Cupones.FindAsync(id);
        }

        public async Task<Cupon> CreateAsync(Cupon cupon)
        {
            _context.Cupones.Add(cupon);
            await _context.SaveChangesAsync();
            return cupon;
        }

        public async Task<Cupon> UpdateAsync(Cupon cupon)
        {
            var existing = await _context.Cupones.FindAsync(cupon.Id);
            if (existing == null) return null!;

            existing.Codigo = cupon.Codigo;
            existing.TipoDescuento = cupon.TipoDescuento;
            existing.ValorDescuento = cupon.ValorDescuento;
            existing.PedidoMinimo = cupon.PedidoMinimo;
            existing.FechaInicio = cupon.FechaInicio;
            existing.FechaFin = cupon.FechaFin;
            existing.LimiteUsos = cupon.LimiteUsos;
            existing.UsosActuales = cupon.UsosActuales;
            existing.Activo = cupon.Activo;
            existing.RestauranteId = cupon.RestauranteId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var cupon = await _context.Cupones.FindAsync(id);
            if (cupon == null) return false;

            _context.Cupones.Remove(cupon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
