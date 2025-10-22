using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Domain.Entities;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Util.IntHashtable;

namespace caAGUAAPI.Infraestructure.Persistence.Repositories
{
    public class PermisoRepository : IPermisoRepository
    {
        private readonly AppDbContext _context;

        public PermisoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Permiso?> GetByIdAsync(int id)
        {
            return await _context.Permisos
                                 .Where(u => u.IdPermiso == id)
                                 .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, Permiso dto)
        {
            var existing = await _context.Permisos
                                 .Where(e => e.IdPermiso == id)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(dto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Permisos
                                 .Where(e => e.IdPermiso == id)
                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            _context.Permisos.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RolConPermisoDTO>> GetRolesDelPermiso(int idPermiso)
        {
            var roles = await _context.RolesPermisos
                .Where(rp => rp.IdPermiso == idPermiso)
                .Select(rp => rp.IdRol)
                .ToListAsync();

            if (roles == null || !roles.Any())
            {
                return null;
            }

            var resultado = new List<RolConPermisoDTO>();

            foreach(var x in roles)
            {
                var componeResultado = await _context.Roles
                    .Where(r => roles.Contains(r.IdRol))
                    .Select(r => new RolDTO
                    {
                        NombreRol = r.NombreRol,
                        Descripcion = r.Descripcion,
                    }).FirstOrDefaultAsync();

                if (componeResultado == null)
                    continue;

                var permisosResultado = await GetPermisosRol(x);

                if(permisosResultado == null || !permisosResultado.Any())
                    continue;

                var subResultado = new RolConPermisoDTO
                {
                    NombreRol = componeResultado.NombreRol,
                    Descripcion = componeResultado.Descripcion,
                    Permisos = permisosResultado
                };

                resultado.Add(subResultado);
            }

            return resultado;
        }

        public async Task<List<string>> GetPermisosRol(int idRol)
        {
            var permisos = _context.RolesPermisos
                                 .Where(rp => rp.IdRol == idRol)
                                 .Select(rp => rp.IdPermiso)
                                 .ToList();

            if (permisos == null || !permisos.Any())
            {
                return null;
            }

            var permisosNombres = _context.Permisos
                .Where(p => permisos.Contains(p.IdPermiso))
                .Select(p => p.NombrePermiso)
                .ToList();

            return permisosNombres;
        }
    }
}
