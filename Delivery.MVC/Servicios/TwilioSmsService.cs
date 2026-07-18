using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Delivery.MVC.Servicios
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public TwilioSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarSmsConfirmacionAsync(string telefono, string codigoVerificacion)
        {
            string mensaje = $"¡Bienvenido a RayoExpres! Tu código de verificación es: {codigoVerificacion}. Expira en 15 minutos.";
            await EnviarSmsAsync(telefono, mensaje);
        }

        public async Task EnviarSmsRecuperacionAsync(string telefono, string codigoRecuperacion)
        {
            string mensaje = $"RayoExpres: Tu código para recuperar la contraseña es: {codigoRecuperacion}. Expira en 15 minutos.";
            await EnviarSmsAsync(telefono, mensaje);
        }

        private async Task EnviarSmsAsync(string destinatario, string mensaje)
        {
            try
            {
                var twilioConfig = _configuration.GetSection("TwilioSettings");
                var accountSid = twilioConfig["AccountSid"];
                var authToken = twilioConfig["AuthToken"];
                var fromNumber = twilioConfig["FromPhoneNumber"];

                if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
                {
                    Console.WriteLine("[CRITICAL] Faltan credenciales de Twilio en la configuración.");
                    return;
                }

                TwilioClient.Init(accountSid, authToken);

                // Asegurar formato internacional para el destinatario
                if (!destinatario.StartsWith("+"))
                {
                    // Asumimos Ecuador por defecto si no tiene código de país
                    if (destinatario.StartsWith("0"))
                    {
                        destinatario = "+593" + destinatario.Substring(1);
                    }
                    else
                    {
                        destinatario = "+593" + destinatario;
                    }
                }

                var messageOptions = new CreateMessageOptions(new PhoneNumber(destinatario))
                {
                    From = new PhoneNumber(fromNumber),
                    Body = mensaje
                };

                var msg = await MessageResource.CreateAsync(messageOptions);
                Console.WriteLine($"[INFO] SMS enviado a {destinatario} exitosamente. SID: {msg.Sid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL] Error al enviar SMS a {destinatario}: {ex.Message}");
                // No lanzamos la excepción para no interrumpir el flujo principal
            }
        }
    }
}
