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
    public class DatosRepository : IDatosRepository
    {
        private readonly AppDbContext _context;

        public DatosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCantidadContribuyentesActivosAsync(int idMunicipio)
        {
            var estadoActivoId = await _context.Estados
                .Where(e =>e.Descripcion == "Activo")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return await _context.Contribuyentes
                .CountAsync(c =>c.IdMunicipio == idMunicipio && c.EstadoId == estadoActivoId);
        }

        public async Task<int> TotalInmueblesRegistradosAsync(int idMunicipio)
        {
            return await _context.Inmuebles.CountAsync(c => c.IdMunicipio == idMunicipio);
        }

        public async Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync(int idMunicipio)
        {
            return await _context.Inmuebles
                .Where(i =>i.IdMunicipio == idMunicipio && i.EstadoId == 1) // Solo inmuebles activos
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

        public async Task<decimal> DeudaTotalAcumuladaAsync(int idMunicipio)
        {
            return await _context.Tributos
                        .Where(t =>t.IdMunicipio == idMunicipio && t.IdEstadoTributo != 1)
                        .SumAsync(t => t.Monto);
        }

        public async Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync(int idMunicipio)
        {
            var resultado = await (from tributo in _context.Tributos
                                   join contribuyente in _context.Contribuyentes
                                       on tributo.IdContribuyente equals contribuyente.Id
                                   where tributo.IdMunicipio == idMunicipio && tributo.IdEstadoTributo != 1
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
