using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Servicios.Interfaces
{
    public interface IAuditoriaService
    {
        Task<IEnumerable<RegistroAuditoria>> GetAllAsync();
        Task<RegistroAuditoria?> GetByIdAsync(long id);
        Task<RegistroAuditoria> CreateAsync(RegistroAuditoria registro);
    }
}
