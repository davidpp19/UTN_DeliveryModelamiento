using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Delivery.MVC.Servicios
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration config, ILogger<SmsService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            var accountSid = _config["TwilioSettings:AccountSid"];
            var authToken = _config["TwilioSettings:AuthToken"];
            var fromNumber = _config["TwilioSettings:FromPhoneNumber"];

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
            {
                _logger.LogWarning("Twilio credentials are not configured properly.");
                return false;
            }

            try
            {
                TwilioClient.Init(accountSid, authToken);

                var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
                {
                    From = new PhoneNumber(fromNumber),
                    Body = message
                };

                var msg = await MessageResource.CreateAsync(messageOptions);
                _logger.LogInformation("SMS sent to {ToPhoneNumber} with Message SID: {Sid}", toPhoneNumber, msg.Sid);
                
                return msg.Status == MessageResource.StatusEnum.Queued || 
                       msg.Status == MessageResource.StatusEnum.Sent || 
                       msg.Status == MessageResource.StatusEnum.Delivered;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {ToPhoneNumber}", toPhoneNumber);
                return false;
            }
        }
    }
}
