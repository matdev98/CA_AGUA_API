using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuarios> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<Usuarios> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CrearUsuarioAsync(Usuarios usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public string ObtenerRolesUsuario(int usuarioId)
        {
            var idrol = _context.UsuariosRoles
                .Where(ur => ur.IdUsuario == usuarioId)
                .Select(ur => ur.IdRol)
                .FirstOrDefault();
            if (idrol == 0)
            {
                return string.Empty;
            }

            return _context.Roles
                .Where(r => r.IdRol == idrol)
                .Select(r => r.NombreRol)
                .FirstOrDefault() ?? string.Empty;
        }

        public List<string> ObtenerPermisosRol(int usuarioId)
        {
            var idrol = _context.UsuariosRoles
                .Where(ur => ur.IdUsuario == usuarioId)
                .Select(ur => ur.IdRol)
                .FirstOrDefault();

            if (idrol == 0)
            {
                return new List<string>();
            }

            var permisos = _context.RolesPermisos
                .Where(rp => rp.IdRol == idrol)
                .Select(rp => rp.IdPermiso)
                .ToList();

            if (permisos.Count == 0)
            {
                return new List<string>();
            }

            return _context.Permisos
                .Where(p => permisos.Contains(p.IdPermiso))
                .Select(p => p.NombrePermiso)
                .ToList();
        }
    }


}
