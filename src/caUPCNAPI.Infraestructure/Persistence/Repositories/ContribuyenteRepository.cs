using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class ContribuyenteRepository : BaseRepository<Contribuyente>, IContribuyenteRepository
    {
        private readonly AppDbContext _context;

        public ContribuyenteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _context.Contribuyentes
                .Where(c => c.IdMunicipio == idMunicipio)
                .ToListAsync();
        }
    }
}
