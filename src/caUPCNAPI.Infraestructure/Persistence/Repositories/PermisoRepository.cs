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
    }
}
