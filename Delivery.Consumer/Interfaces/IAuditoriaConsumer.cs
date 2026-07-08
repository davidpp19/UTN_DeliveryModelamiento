using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Modelos.Entidades;

namespace Delivery.Consumer.Interfaces
{
    public interface IAuditoriaConsumer
    {
        Task<IEnumerable<RegistroAuditoria>> GetAllAsync();
        Task<RegistroAuditoria?> GetByIdAsync(long id);
        Task<RegistroAuditoria> CreateAsync(RegistroAuditoria entity);
    }
}
