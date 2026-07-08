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

        public UsuarioService(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.Rol).ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
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
            existingUsuario.PasswordHash = usuario.PasswordHash;
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
