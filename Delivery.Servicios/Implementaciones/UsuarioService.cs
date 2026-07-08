using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Delivery.Modelos;
using Delivery.Modelos.Entidades;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DeliveryDbContext _context;
        private readonly ISeguridadService _seguridadService;

        public UsuarioService(DeliveryDbContext context, ISeguridadService seguridadService)
        {
            _context = context;
            _seguridadService = seguridadService;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.Rol).ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            usuario.PasswordHash = _seguridadService.HashearPassword(usuario.PasswordHash);
            usuario.CreadoEn = System.DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.Id);
            if (existingUsuario == null) return null!;

            existingUsuario.RolId = usuario.RolId;
            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Apellidos = usuario.Apellidos;
            existingUsuario.Email = usuario.Email;
            
            // Si la contraseña cambió (no es hash), la hasheamos.
            if (!usuario.PasswordHash.StartsWith("$2a$") && !usuario.PasswordHash.StartsWith("$2b$") && !usuario.PasswordHash.StartsWith("$2y$"))
            {
                existingUsuario.PasswordHash = _seguridadService.HashearPassword(usuario.PasswordHash);
            }

            existingUsuario.Telefono = usuario.Telefono;
            existingUsuario.TipoUsuario = usuario.TipoUsuario;
            existingUsuario.Activo = usuario.Activo;
            existingUsuario.FotoPerfilUrl = usuario.FotoPerfilUrl;
            existingUsuario.ActualizadoEn = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingUsuario;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
