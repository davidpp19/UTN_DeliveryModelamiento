using System.Threading.Tasks;

namespace Delivery.MVC.Servicios
{
    public interface ISmsService
    {
        Task EnviarSmsConfirmacionAsync(string telefono, string codigoVerificacion);
        Task EnviarSmsRecuperacionAsync(string telefono, string codigoRecuperacion);
    }
}
