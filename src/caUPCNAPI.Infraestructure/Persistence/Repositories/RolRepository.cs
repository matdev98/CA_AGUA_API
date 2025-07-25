using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Util.IntHashtable;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await _context.Roles
                                 .Where(u => u.IdRol == id)
                                 .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, Rol dto)
        {
            var existing = await _context.Roles
                                 .Where(e => e.IdRol == id)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(dto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Roles
                                 .Where(e => e.IdRol == id)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.Roles.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserRolAsync(int idUsuario, int idRol)
        {
            var existing = await _context.UsuariosRoles
                                 .Where(e => e.IdRol == idRol && e.IdUsuario == idUsuario)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.UsuariosRoles.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRolPermisoAsync(int idRol, int idPermiso)
        {
            var existing = await _context.RolesPermisos
                                 .Where(e => e.IdRol == idRol && e.IdPermiso == idPermiso)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.RolesPermisos.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetPermisosRol(int idRol)
        {
            var permisos = await _context.RolesPermisos
                                 .Where(rp => rp.IdRol == idRol)
                                 .Select(rp => rp.IdPermiso)
                                 .ToListAsync();

            if (permisos == null || !permisos.Any())
            {
                return null;
            }

            var permisosNombres = await _context.Permisos
                .Where(p => permisos.Contains(p.IdPermiso))
                .Select(p => p.NombrePermiso)
                .ToListAsync();

            return permisosNombres;
        }

        public async Task<List<UserDTO>> GetUsersByRol(int idRol)
        {
            var users = await _context.UsuariosRoles
                .Where(ur => ur.IdRol == idRol)
                .Select(ur => ur.IdUsuario)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return null;
            }

            var userDetails = await _context.Usuarios
                .Where(u => users.Contains(u.Id))
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    NombreUsuario = u.NombreUsuario,
                    Email = u.Email,
                    NombreCompleto = u.NombreCompleto,
                    Activo = u.Activo,
                    IdMunicipio = u.IdMunicipio
                })
                .ToListAsync();

            return userDetails;
        }
    }
}
