using System.Threading.Tasks;
using Delivery.Modelos.DTOs;

namespace Delivery.Consumer.Interfaces
{
    public interface IAuthConsumer
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> RecuperarPasswordAsync(string email);
        Task<AuthResponseDto?> RegistroRepartidorAsync(RegistroRepartidorDto dto);
        Task<AuthResponseDto?> RegistroRestauranteAsync(RegistroRestauranteDto dto);
    }
}
