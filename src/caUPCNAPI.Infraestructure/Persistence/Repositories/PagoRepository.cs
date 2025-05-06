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
    public class PagoRepository : BaseRepository<Pago>, IPagoRepository
    {
        private readonly AppDbContext _context;

        public PagoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Pago>> PagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            var existe = await _context.Contribuyentes.AnyAsync(c => c.Id == idContribuyente);
            if (!existe) return new List<Pago>();

            var pagos = await _context.Pagos
                .Where(p => p.IdContribuyente == idContribuyente
                            && p.Idinmueble == idInmueble
                            && p.Periodo == periodo)
                .ToListAsync();

            return pagos;
        }


    }
}
