using System.Threading.Tasks;

namespace Delivery.MVC.Servicios
{
    public interface ISmsService
    {
        Task<string> EnviarSmsConfirmacionAsync(string telefono, string codigoVerificacion);
        Task<string> EnviarSmsRecuperacionAsync(string telefono, string codigoRecuperacion);
    }
}
