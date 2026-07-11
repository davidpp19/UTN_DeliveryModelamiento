using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class GeolocalizacionService : IGeolocalizacionService
    {
        private readonly DeliveryDbContext _context;

        public GeolocalizacionService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarUbicacionRepartidorAsync(ActualizacionUbicacionDto actualizacion)
        {
            var ubicacion = await _context.Set<UbicacionActualRepartidor>()
                                          .FirstOrDefaultAsync(u => u.RepartidorId == actualizacion.RepartidorId);

            if (ubicacion == null)
            {
                ubicacion = new UbicacionActualRepartidor
                {
                    RepartidorId = actualizacion.RepartidorId,
                    Latitud = actualizacion.Ubicacion.Latitud,
                    Longitud = actualizacion.Ubicacion.Longitud,
                    ActualizadoEn = actualizacion.Timestamp
                };
                _context.Set<UbicacionActualRepartidor>().Add(ubicacion);
            }
            else
            {
                ubicacion.Latitud = actualizacion.Ubicacion.Latitud;
                ubicacion.Longitud = actualizacion.Ubicacion.Longitud;
                ubicacion.ActualizadoEn = actualizacion.Timestamp;
                _context.Set<UbicacionActualRepartidor>().Update(ubicacion);
            }

            // En un futuro: Notificar vía SignalR, WebSockets, Firebase, Mapbox, etc.
            
            await _context.SaveChangesAsync();
        }

        public async Task<CoordenadasDto?> ObtenerUbicacionRepartidorAsync(long repartidorId)
        {
            var ubicacion = await _context.Set<UbicacionActualRepartidor>()
                                          .FirstOrDefaultAsync(u => u.RepartidorId == repartidorId);

            if (ubicacion == null) return null;

            return new CoordenadasDto
            {
                Latitud = ubicacion.Latitud ?? 0m,
                Longitud = ubicacion.Longitud ?? 0m
            };
        }

        public double CalcularDistanciaKm(double lat1, double lon1, double lat2, double lon2)
        {
            var r = 6371; // Radio de la Tierra en kilómetros
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public async Task<(double DistanciaKm, double TiempoMinutos)> CalcularRutaOSRMAsync(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                // OSRM espera las coordenadas en formato: longitud,latitud
                var url = $"http://router.project-osrm.org/route/v1/driving/{lon1.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lat1.ToString(System.Globalization.CultureInfo.InvariantCulture)};{lon2.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lat2.ToString(System.Globalization.CultureInfo.InvariantCulture)}?overview=false";
                
                client.DefaultRequestHeaders.Add("User-Agent", "RayoExpresDeliveryApp/1.0");

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    
                    var code = doc.RootElement.GetProperty("code").GetString();
                    if (code == "Ok")
                    {
                        var routes = doc.RootElement.GetProperty("routes");
                        if (routes.GetArrayLength() > 0)
                        {
                            var route = routes[0];
                            var distanceMeters = route.GetProperty("distance").GetDouble();
                            var durationSeconds = route.GetProperty("duration").GetDouble();
                            
                            return (distanceMeters / 1000.0, durationSeconds / 60.0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback en caso de error de red o de OSRM
                Console.WriteLine($"Error al consultar OSRM: {ex.Message}");
            }

            // Fallback: Haversine distance, y un tiempo estimado asumiendo 30 km/h (0.5 km/min)
            var haversine = CalcularDistanciaKm(lat1, lon1, lat2, lon2);
            return (haversine, haversine * 2); 
        }

        public decimal CalcularCostoEnvio(double distanciaKm)
        {
            // Redondear la distancia al km superior (ej: 2.1km -> 3km, 3.8km -> 4km)
            var distanciaRedondeada = Math.Ceiling(distanciaKm);

            decimal costoBase = 1.50m;
            int distanciaBase = 3;
            decimal costoPorKmAdicional = 0.25m;

            if (distanciaRedondeada <= distanciaBase)
            {
                return costoBase;
            }

            var kmAdicionales = (decimal)(distanciaRedondeada - distanciaBase);
            return costoBase + (kmAdicionales * costoPorKmAdicional);
        }
    }
}
