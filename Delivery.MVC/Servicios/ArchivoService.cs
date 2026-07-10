using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Delivery.MVC.Servicios
{
    public class ArchivoService : IArchivoService
    {
        private readonly IWebHostEnvironment _env;

        public ArchivoService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> GuardarArchivoAsync(IFormFile archivo, string subCarpeta)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            // Validar extensión
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                throw new ArgumentException("Extensión de archivo no permitida. Solo JPG, PNG, WEBP.");

            // Validar tamaño (Max 5MB)
            if (archivo.Length > 5 * 1024 * 1024)
                throw new ArgumentException("El archivo excede el tamaño máximo permitido de 5MB.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subCarpeta);
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var nombreUnico = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(uploadsFolder, nombreUnico);

            using (var fileStream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            // Devolver la ruta relativa web (para src de imgs)
            return $"/uploads/{subCarpeta}/{nombreUnico}";
        }

        public bool EliminarArchivo(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
                return false;

            // Quitar el '/' inicial si existe
            var rutaRelativaLimpia = rutaRelativa.TrimStart('/');
            var rutaFisica = Path.Combine(_env.WebRootPath, rutaRelativaLimpia);

            if (File.Exists(rutaFisica))
            {
                File.Delete(rutaFisica);
                return true;
            }

            return false;
        }
    }
}
