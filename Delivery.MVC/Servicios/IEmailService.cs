namespace Delivery.MVC.Servicios
{
    public interface IEmailService
    {
        Task EnviarCorreoConfirmacionAsync(string email, string nombre, string codigoVerificacion);
        Task EnviarCorreoRecuperacionAsync(string email, string nombre, string codigoRecuperacion);
    }
}
