using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Delivery.MVC.Servicios
{
    public class ArchivoService : IArchivoService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly string? _blobConnectionString;

        public ArchivoService(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
            _blobConnectionString = _configuration["BlobStorageConnectionString"];
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

            var nombreUnico = $"{Guid.NewGuid()}{extension}";

            // Si hay conexión a Blob Storage, usar Azure
            if (!string.IsNullOrEmpty(_blobConnectionString))
            {
                var blobServiceClient = new BlobServiceClient(_blobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("archivos");
                
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                var blobClient = containerClient.GetBlobClient($"{subCarpeta.ToLower()}/{nombreUnico}");

                using (var stream = archivo.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = archivo.ContentType });
                }

                return blobClient.Uri.ToString();
            }
            else
            {
                // Modo local
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subCarpeta);
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var rutaFisica = Path.Combine(uploadsFolder, nombreUnico);

                using (var fileStream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await archivo.CopyToAsync(fileStream);
                }

                // Devolver la ruta relativa web (para src de imgs)
                return $"/uploads/{subCarpeta}/{nombreUnico}";
            }
        }

        public bool EliminarArchivo(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
                return false;

            // Identificar si es una URL de Azure o ruta local
            if (rutaRelativa.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(_blobConnectionString))
                    return false; // No hay cadena de conexión, no se puede borrar de Azure

                try
                {
                    var uri = new Uri(rutaRelativa);
                    var segments = uri.Segments;
                    if (segments.Length >= 3)
                    {
                        var containerName = segments[1].TrimEnd('/');
                        var blobName = string.Join("", segments.Skip(2));

                        var blobServiceClient = new BlobServiceClient(_blobConnectionString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        var blobClient = containerClient.GetBlobClient(blobName);
                        
                        blobClient.DeleteIfExists();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // Ruta local
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
}
