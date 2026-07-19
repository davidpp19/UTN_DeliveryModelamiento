using System.Threading.Tasks;

namespace Delivery.MVC.Servicios
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    }
}
