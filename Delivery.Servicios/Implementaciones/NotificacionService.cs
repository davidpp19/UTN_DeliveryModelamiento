using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class NotificacionService : INotificacionService
    {
        private readonly DeliveryDbContext _context;

        public NotificacionService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task EnviarNotificacionAsync(MensajeNotificacionDto mensaje)
        {
            // Insertar en la BD para que quede registrada
            var entity = new Notificacion
            {
                UsuarioId = mensaje.UsuarioDestinoId,
                Titulo = mensaje.Asunto,
                Mensaje = mensaje.Contenido,
                CreadaEn = DateTime.UtcNow,
                Leida = false
            };

            _context.Notificaciones.Add(entity);
            await _context.SaveChangesAsync();

            // Aquí en un futuro se enviaría el email, SMS o push real.
        }

        public async Task EnviarNotificacionMultiCanalAsync(MensajeNotificacionDto mensaje, params Delivery.Modelos.Enums.CanalNotificacionEnum[] canales)
        {
            // Almacenamos una sola vez en BD, asumiendo que los canales son medios de entrega, pero la notificación lógica es una sola.
            var entity = new Notificacion
            {
                UsuarioId = mensaje.UsuarioDestinoId,
                Titulo = mensaje.Asunto,
                Mensaje = mensaje.Contenido,
                CreadaEn = DateTime.UtcNow,
                Leida = false
            };
            
            _context.Notificaciones.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificacionDto>> GetNotificacionesByUsuarioAsync(long usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.CreadaEn)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Titulo = n.Titulo,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    CreadaEn = n.CreadaEn
                })
                .ToListAsync();
        }

        public async Task MarcarComoLeidaAsync(long notificacionId, long usuarioId)
        {
            var notificacion = await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificacionId && n.UsuarioId == usuarioId);
            if (notificacion != null)
            {
                notificacion.Leida = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
