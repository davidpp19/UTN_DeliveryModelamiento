using System.Threading.Tasks;

namespace Delivery.Servicios.Interfaces
{
    public interface ISeguridadService
    {
        string HashearPassword(string password);
        bool VerificarPassword(string password, string hash);
        string GenerarTokenJwt(long usuarioId, string email, string rol);
    }
}
