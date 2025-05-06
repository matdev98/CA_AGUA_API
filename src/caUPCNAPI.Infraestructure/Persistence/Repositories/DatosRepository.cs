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
    public class DatosRepository : IDatosRepository
    {
        private readonly AppDbContext _context;

        public DatosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCantidadContribuyentesActivosAsync()
        {
            var estadoActivoId = await _context.Estados
                .Where(e => e.Descripcion == "Activo")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return await _context.Contribuyentes
                .CountAsync(c => c.EstadoId == estadoActivoId);
        }

        public async Task<int> TotalInmueblesRegistradosAsync()
        {
            return await _context.Inmuebles.CountAsync();
        }

        public async Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync()
        {
            return await _context.Inmuebles
                .Where(i => i.EstadoId == 1) // Solo inmuebles activos
                .Join(_context.TiposInmueble,
                        inmueble => inmueble.IdTipoInmueble,
                        tipo => tipo.Id,
                        (inmueble, tipo) => new { tipo.Descripcion })
                .GroupBy(x => x.Descripcion)
                .Select(g => new InmueblesPorTipoDTO
                {
                    Tipo = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();
        }

        public async Task<decimal> DeudaTotalAcumuladaAsync()
        {
            return await _context.Tributos
                        .Where(t => t.IdEstadoTributo != 1)
                        .SumAsync(t => t.Monto);
        }

        public async Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync()
        {
            var resultado = await (from tributo in _context.Tributos
                                   join contribuyente in _context.Contribuyentes
                                       on tributo.IdContribuyente equals contribuyente.Id
                                   where tributo.IdEstadoTributo != 1
                                   group new { tributo, contribuyente } by new
                                   {
                                       contribuyente.Id,
                                       contribuyente.Nombres,
                                       contribuyente.Apellidos,
                                       contribuyente.NumeroDocumento
                                   } into g
                                   orderby g.Sum(x => x.tributo.Monto) descending
                                   select new TopDeudoresDTO
                                   {
                                       IdContribuyente = g.Key.Id,
                                       NombreCompleto = g.Key.Apellidos + ", " + g.Key.Nombres,
                                       Documento = g.Key.NumeroDocumento,
                                       DeudaTotal = g.Sum(x => x.tributo.Monto)
                                   })
                          .Take(10)
                          .ToListAsync();

            return resultado;

        }



    }
}
