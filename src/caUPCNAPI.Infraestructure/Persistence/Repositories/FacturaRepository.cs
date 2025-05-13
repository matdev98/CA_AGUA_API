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
    public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
    {
        private readonly AppDbContext _context;

        public FacturaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Factura> GetByContribuyenteAndPeriodoAsync(int idContribuyente, string periodo)
        {
            return await _context.Factura
                .FirstOrDefaultAsync(f => f.IdContribuyente == idContribuyente && f.Periodo == periodo);
        }
    }


}
