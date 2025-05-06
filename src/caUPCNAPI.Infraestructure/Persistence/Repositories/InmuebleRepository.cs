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
    public class InmuebleRepository : BaseRepository<Inmueble>, IInmuebleRepository
    {
        private readonly AppDbContext _context;

        public InmuebleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            var inmuebles = await _context.Inmuebles
                .Where(i => i.IdContribuyente == contribuyenteId)
                .ToListAsync();

            var resultado = inmuebles.Select(i => new Inmueble
            {
                Id = i.Id,
                IdContribuyente = i.IdContribuyente,
                IdTipoInmueble = i.IdTipoInmueble,
                IdLocalidad = i.IdLocalidad,
                EvaluoFiscal = i.EvaluoFiscal,
                EstadoId = i.EstadoId,
                Referencias = i.Referencias,
                Calle = i.Calle,
                Numero = i.Numero,
                Orientacion = i.Orientacion,
                AreaTotal = i.AreaTotal
            });

            return resultado;
        }
    }

}
