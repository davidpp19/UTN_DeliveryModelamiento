using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Delivery.MVC.Servicios
{
    public interface IArchivoService
    {
        Task<string?> GuardarArchivoAsync(IFormFile archivo, string subCarpeta);
        bool EliminarArchivo(string rutaRelativa);
    }
}
