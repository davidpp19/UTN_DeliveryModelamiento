using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly DeliveryDbContext _context;

        public AuditoriaService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistroAuditoria>> GetAllAsync()
        {
            return await _context.RegistrosAuditoria.ToListAsync();
        }

        public async Task<RegistroAuditoria?> GetByIdAsync(long id)
        {
            return await _context.RegistrosAuditoria.FindAsync(id);
        }

        public async Task<RegistroAuditoria> CreateAsync(RegistroAuditoria registro)
        {
            if (registro.FechaHora == default)
            {
                registro.FechaHora = DateTime.UtcNow;
            }
            _context.RegistrosAuditoria.Add(registro);
            await _context.SaveChangesAsync();
            return registro;
        }
    }
}
