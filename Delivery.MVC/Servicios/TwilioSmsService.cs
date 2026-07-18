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

        public async Task<string> EnviarSmsConfirmacionAsync(string telefono, string codigoVerificacion)
        {
            string mensaje = $"¡Bienvenido a RayoExpres! Tu código de verificación es: {codigoVerificacion}. Expira en 15 minutos.";
            return await EnviarSmsAsync(telefono, mensaje);
        }

        public async Task<string> EnviarSmsRecuperacionAsync(string telefono, string codigoRecuperacion)
        {
            string mensaje = $"RayoExpres: Tu código para recuperar la contraseña es: {codigoRecuperacion}. Expira en 15 minutos.";
            return await EnviarSmsAsync(telefono, mensaje);
        }

        private async Task<string> EnviarSmsAsync(string destinatario, string mensaje)
        {
            try
            {
                var twilioConfig = _configuration.GetSection("TwilioSettings");
                var accountSid = twilioConfig["AccountSid"];
                var authToken = twilioConfig["AuthToken"];
                var fromNumber = twilioConfig["FromPhoneNumber"];

                if (string.IsNullOrEmpty(accountSid) || accountSid == "YOUR_TWILIO_ACCOUNT_SID_AQUI")
                {
                    return "Faltan credenciales de Twilio en la configuración de Azure.";
                }

                TwilioClient.Init(accountSid, authToken);

                // Asegurar formato internacional para el destinatario
                if (!destinatario.StartsWith("+"))
                {
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
                return string.Empty; // Éxito
            }
            catch (Exception ex)
            {
                return $"Error de Twilio: {ex.Message}";
            }
        }
    }
}
