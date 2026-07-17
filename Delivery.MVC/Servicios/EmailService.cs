using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Delivery.MVC.Servicios
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoConfirmacionAsync(string email, string nombre, string codigoVerificacion)
        {
            string asunto = "Confirma tu cuenta en RayoExpres";
            string html = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #FF6600;'>¡Bienvenido a RayoExpres, {nombre}!</h2>
                    <p>Gracias por registrarte. Para activar tu cuenta, por favor ingresa el siguiente código de verificación:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; font-size: 24px; font-weight: bold; letter-spacing: 5px; text-align: center; margin: 20px 0;'>
                        {codigoVerificacion}
                    </div>
                    <p>Este código expirará en 15 minutos.</p>
                    <p>Si no solicitaste este registro, ignora este correo.</p>
                </div>
            ";
            await EnviarCorreoAsync(email, asunto, html);
        }

        public async Task EnviarCorreoRecuperacionAsync(string email, string nombre, string codigoRecuperacion)
        {
            string asunto = "Recuperación de contraseña en RayoExpres";
            string html = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #FF6600;'>Hola, {nombre}</h2>
                    <p>Hemos recibido una solicitud para restablecer tu contraseña. Usa el siguiente código:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; font-size: 24px; font-weight: bold; letter-spacing: 5px; text-align: center; margin: 20px 0;'>
                        {codigoRecuperacion}
                    </div>
                    <p>Este código expirará en 15 minutos.</p>
                    <p>Si no solicitaste cambiar tu contraseña, puedes ignorar este correo de forma segura.</p>
                </div>
            ";
            await EnviarCorreoAsync(email, asunto, html);
        }

        private async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var emailConfig = _configuration.GetSection("SmtpSettings");
                var server = emailConfig["Server"] ?? "smtp.gmail.com";
                var portStr = emailConfig["Port"] ?? "587";
                int port = int.TryParse(portStr, out int p) ? p : 587;
                var senderName = emailConfig["SenderName"] ?? "RayoExpres";
                var senderEmail = emailConfig["SenderEmail"] ?? "kdpenap@utn.edu.ec";
                var username = emailConfig["Username"] ?? "kdpenap@utn.edu.ec";
                var password = emailConfig["Password"] ?? "numa omlm zkit lvvr";

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(new MailboxAddress("", destinatario));
                email.Subject = asunto;

                var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                // Aceptar cualquier certificado temporalmente si hay problemas de SSL en Azure
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(server, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"[CRITICAL] Error al enviar correo SMTP a {destinatario}: {ex.Message}");
                // No lanzamos la excepción para no romper el flujo principal (ej. registro).
                // El usuario podrá solicitar reenviar el código.
            }
        }
    }
}
