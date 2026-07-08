using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Servicios.Interfaces
{
    public interface IPasarelaPagoService
    {
        Task<RespuestaPagoDto> ProcesarPagoAsync(SolicitudPagoDto solicitud);
        Task<RespuestaPagoDto> ReembolsarPagoAsync(string transaccionId, decimal monto);
    }
}
