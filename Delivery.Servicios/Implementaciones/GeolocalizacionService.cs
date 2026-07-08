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
    }
}
