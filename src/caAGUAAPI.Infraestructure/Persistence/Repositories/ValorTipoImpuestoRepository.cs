using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Infraestructure.Persistence.Repositories
{
    public class ValorTipoImpuestoRepository : BaseRepository<ValorTipoImpuesto>, IValorTipoImpuestoRepository
    {
        private readonly AppDbContext _context;

        public ValorTipoImpuestoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NombreTipoImpuestoDTO>> GetNombreTipoImpuesto()
        {
            var result = from vti in _context.ValoresTipoImpuesto
                         join ti in _context.TiposImpuesto on vti.TipoImpuestoId equals ti.Id
                         select new NombreTipoImpuestoDTO
                         {
                             TipoImpuestoId = ti.Id,
                             NombreTipoImpuesto = ti.Descripcion,
                             PeriodoDesde = vti.PeriodoDesde,
                             PeriodoHasta = vti.PeriodoHasta,
                             Valor = vti.Valor,
                             Resolucion = vti.Resolucion
                         };

            return await result.ToListAsync();
        }

    }

}
