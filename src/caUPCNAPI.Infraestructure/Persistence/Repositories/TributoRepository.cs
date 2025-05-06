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
    public class TributoRepository : BaseRepository<Tributo>, ITributoRepository
    {
        private readonly AppDbContext _context;

        public TributoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TributoContribuyenteDTO>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            var hoy = DateTime.Today;
            var tributos = new List<TributoContribuyenteDTO>();

            var contribuyente = await _context.Contribuyentes.FindAsync(contribuyenteId);
            if (contribuyente == null) return tributos;

            var inmuebles = await _context.Inmuebles
                .Where(i => i.IdContribuyente == contribuyenteId)
                .ToListAsync();

            var tipoImpuestoFijo = await _context.TiposImpuesto.FirstOrDefaultAsync(t => t.Id == 1);
            var valorFijo = await _context.ValoresTipoImpuesto.FirstOrDefaultAsync(v => v.TipoImpuestoId == 1);

            foreach (var inmueble in inmuebles)
            {
                // Siempre agregar impuesto fijo
                if (tipoImpuestoFijo != null && valorFijo != null)
                {
                    decimal montoFijo = inmueble.AreaTotal * valorFijo.Valor;
                    DateTime fechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(9);

                    tributos.Add(new TributoContribuyenteDTO
                    {
                        Id = 0,
                        IdMunicipio = tipoImpuestoFijo.MunicipioId,
                        IdContribuyente = contribuyente.Id,
                        Nombre = contribuyente.Nombres.Trim(),
                        Apellido = contribuyente.Apellidos.Trim(),
                        Direccion = $"{inmueble.Calle} {inmueble.Numero}",
                        Documento = contribuyente.NumeroDocumento.Trim(),
                        IdInmueble = inmueble.Id,
                        IdTipoImpuesto = tipoImpuestoFijo.Id,
                        Descripcion = tipoImpuestoFijo.Descripcion.Trim(),
                        Periodo = "",
                        Monto = montoFijo,
                        FechaEmision = hoy,
                        FechaVencimiento = fechaVencimiento,
                        IdEstado = 1,
                        EstadoTributoDescripcion = "Pendiente" // Podés actualizar este campo con lógica de pagos si la tenés
                    });
                }

                // Agregar impuestos variables vigentes
                var impuestosVariables = await _context.ContribuyentesImpuestosVariables
                    .Where(iv => iv.IdContribuyente == contribuyenteId
                        && iv.IdInmueble == inmueble.Id
                        && hoy >= iv.PeriodoDesde
                        && hoy <= iv.PeriodoHasta)
                    .ToListAsync();

                foreach (var variable in impuestosVariables)
                {
                    var tipoImpuesto = await _context.TiposImpuesto.FirstOrDefaultAsync(t => t.Id == variable.IdTipoImpuesto);
                    var valorVariable = await _context.ValoresTipoImpuesto.FirstOrDefaultAsync(v => v.TipoImpuestoId == variable.IdTipoImpuesto);

                    if (tipoImpuesto == null || valorVariable == null)
                        continue;

                    decimal monto = valorVariable.Valor;
                    DateTime fechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(9);

                    tributos.Add(new TributoContribuyenteDTO
                    {
                        Id = variable.Id,
                        IdMunicipio = tipoImpuesto.MunicipioId,
                        IdContribuyente = contribuyente.Id,
                        Nombre = contribuyente.Nombres.Trim(),
                        Apellido = contribuyente.Apellidos.Trim(),
                        Direccion = $"{inmueble.Calle} {inmueble.Numero}",
                        Documento = contribuyente.NumeroDocumento.Trim(),
                        IdInmueble = inmueble.Id,
                        IdTipoImpuesto = tipoImpuesto.Id,
                        Descripcion = tipoImpuesto.Descripcion.Trim(),
                        Periodo = $"{variable.PeriodoDesde:yyyy-MM-dd} al {variable.PeriodoHasta:yyyy-MM-dd}",
                        Monto = monto,
                        FechaEmision = hoy,
                        FechaVencimiento = fechaVencimiento,
                        IdEstado = 1,
                        EstadoTributoDescripcion = "Pendiente" // Lógica de pagos también puede ir aquí
                    });
                }
            }

            return tributos;
        }

        public async Task<List<TributoContribuyenteDTO>> ObtenerTributosPorPeriodoAsync(string periodo, int idMunicipio)
        {
            var resultado = await (from tributo in _context.Tributos
                                   join contribuyente in _context.Contribuyentes
                                       on tributo.IdContribuyente equals contribuyente.Id
                                   join inmueble in _context.Inmuebles
                                       on tributo.IdInmueble equals inmueble.Id
                                   join pago in _context.Pagos
                                       on tributo.Id equals pago.IdTributo into pagosGroup
                                   from pago in pagosGroup.DefaultIfEmpty()
                                   where tributo.Periodo == periodo && tributo.IdMunicipio == idMunicipio
                                   select new TributoContribuyenteDTO
                                   {
                                       Id = tributo.Id,
                                       IdMunicipio = tributo.IdMunicipio,
                                       IdContribuyente = contribuyente.Id,
                                       Nombre = contribuyente.Nombres.Trim(),
                                       Apellido = contribuyente.Apellidos.Trim(),
                                       Direccion = inmueble.Calle + " " + inmueble.Numero + " " + inmueble.Orientacion,
                                       Documento = contribuyente.NumeroDocumento.Trim(),
                                       IdInmueble = tributo.IdInmueble,
                                       IdTipoImpuesto = tributo.IdTipoImpuesto,
                                       Descripcion = tributo.Descripcion.Trim(),
                                       Periodo = tributo.Periodo,
                                       Monto = tributo.Monto,
                                       FechaEmision = tributo.FechaEmision,
                                       FechaVencimiento = tributo.FechaVencimiento,
                                       IdEstado = tributo.IdEstado,
                                       EstadoTributoDescripcion =
                                           pago != null && pago.MontoPagado.HasValue && pago.MontoPagado.Value >= tributo.Monto ? "Pagada" :
                                           pago != null && pago.MontoPagado.HasValue && pago.MontoPagado.Value < tributo.Monto && tributo.FechaVencimiento < DateTime.Now ? "Parcial/Vencida" :
                                           pago != null && pago.MontoPagado.HasValue && pago.MontoPagado.Value < tributo.Monto ? "Parcial" :
                                           pago == null && tributo.FechaVencimiento < DateTime.Now ? "Vencida" :
                                           "Pendiente"
                                   }).ToListAsync();



            return resultado;

        }

        public async Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupados(int idContribuyente, string periodo)
        {
            var contribuyente = await _context.Contribuyentes
        .FirstOrDefaultAsync(c => c.Id == idContribuyente);

            if (contribuyente == null)
                return new List<TributoAgrupadoDTO>();

            var tributos = await _context.Tributos
                .Where(t => t.IdContribuyente == idContribuyente && t.Periodo == periodo)
                .ToListAsync();

            var inmueblesIds = tributos.Select(t => t.IdInmueble).Distinct().ToList();

            var inmuebles = await _context.Inmuebles
                .Where(i => inmueblesIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, i => i);

            var pagosSet = await _context.Pagos
                .Where(p => p.Periodo == periodo && inmueblesIds.Contains(p.Idinmueble))
                .Select(p => p.Idinmueble)
                .ToListAsync();

            // Obtener descripciones desde la tabla EstadoTributo
            var estadosTributo = await _context.EstadoTributos
                .Where(e => e.Id == 1 || e.Id == 6)
                .ToDictionaryAsync(e => e.Id, e => e.Descripcion);

            var agrupados = tributos
                .GroupBy(t => t.IdInmueble)
                .Select(g =>
                {
                    var primerTributo = g.First();
                    var inmueble = inmuebles.GetValueOrDefault(g.Key);
                    var estaPagado = pagosSet.Contains(g.Key);

                    var estadoId = estaPagado ? 1 : 6;
                    var estadoDescripcion = estadosTributo.TryGetValue(estadoId, out var desc)
                        ? desc
                        : "Desconocido";

                    return new TributoAgrupadoDTO
                    {
                        IdMunicipio = primerTributo.IdMunicipio,
                        IdContribuyente = primerTributo.IdContribuyente,
                        Nombre = contribuyente.Nombres,
                        Apellido = contribuyente.Apellidos,
                        Direccion = inmueble != null ? $"{inmueble.Calle} {inmueble.Numero}" : "",
                        Documento = contribuyente.NumeroDocumento,
                        IdInmueble = g.Key,
                        Periodo = periodo,
                        Monto = g.Sum(t => t.Monto),
                        IdEstado = estadoId,
                        EstadoTributoDescripcion = estadoDescripcion
                    };
                }).ToList();

            return agrupados;
        }

        public async Task<List<TributoContribuyenteDTO>> ObtenerDetalleTributoPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            var resultado = await (
                from tributo in _context.Tributos
                join contribuyente in _context.Contribuyentes on tributo.IdContribuyente equals contribuyente.Id
                join inmueble in _context.Inmuebles on tributo.IdInmueble equals inmueble.Id
                join estadoTributo in _context.EstadoTributos on tributo.IdEstadoTributo equals estadoTributo.Id
                where tributo.IdContribuyente == idContribuyente
                      && tributo.IdInmueble == idInmueble
                      && tributo.Periodo == periodo
                select new TributoContribuyenteDTO
                {
                    Id = tributo.Id,
                    IdMunicipio = tributo.IdMunicipio,
                    IdContribuyente = contribuyente.Id,
                    Nombre = contribuyente.Nombres.Trim(),
                    Apellido = contribuyente.Apellidos.Trim(),
                    Direccion = inmueble.Calle + ' ' + inmueble.Numero + ' ' + inmueble.Orientacion,
                    Documento = contribuyente.NumeroDocumento.Trim(),
                    IdInmueble = inmueble.Id,
                    IdTipoImpuesto = tributo.IdTipoImpuesto,
                    Descripcion = tributo.Descripcion.Trim(),
                    Periodo = tributo.Periodo,
                    Monto = tributo.Monto,
                    FechaEmision = tributo.FechaEmision,
                    FechaVencimiento = tributo.FechaVencimiento,
                    IdEstado = tributo.IdEstado,
                    EstadoTributoDescripcion = estadoTributo.Descripcion.Trim()
                }
            ).ToListAsync();

            return resultado;
        }


        public async Task GenerarTributosDelMesAsync(int IdMunicipio)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var periodoActual = DateTime.Now.ToString("yyyyMM");

                // Traemos todos los contribuyentes
                var contribuyentes = await _context.Contribuyentes.ToListAsync();

                foreach (var contribuyente in contribuyentes)
                {
                    var inmuebles = await _context.Inmuebles
                        .Where(i => i.IdContribuyente == contribuyente.Id)
                        .ToListAsync();

                    foreach (var inmueble in inmuebles)
                    {
                        // Impuesto fijo
                        var tipoImpuestoFijoId = 1; // ID de impuesto fijo (lo dejamos fijo)

                        var valorFijo = await _context.ValoresTipoImpuesto
                            .Where(v => v.TipoImpuestoId == tipoImpuestoFijoId)
                            .OrderByDescending(v => v.PeriodoDesde)
                            .FirstOrDefaultAsync();

                        var tipoImpuestoFijo = await _context.TiposImpuesto
                            .FirstOrDefaultAsync(t => t.Id == tipoImpuestoFijoId);

                        if (valorFijo != null && tipoImpuestoFijo != null)
                        {
                            // Validar que no exista ya este tributo
                            bool existeTributoFijo = await _context.Tributos.AnyAsync(t =>
                                t.IdContribuyente == contribuyente.Id &&
                                t.IdInmueble == inmueble.Id &&
                                t.IdTipoImpuesto == tipoImpuestoFijoId &&
                                t.Periodo == periodoActual
                            );

                            if (!existeTributoFijo)
                            {
                                var tributoFijo = new Tributo
                                {
                                    IdMunicipio = IdMunicipio,
                                    IdContribuyente = contribuyente.Id,
                                    IdInmueble = inmueble.Id,
                                    IdTipoImpuesto = tipoImpuestoFijoId,
                                    Descripcion = tipoImpuestoFijo.Descripcion,
                                    Periodo = periodoActual,
                                    Monto = valorFijo.Valor * inmueble.AreaTotal,
                                    FechaEmision = DateTime.Now,
                                    FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 10),
                                    IdEstado = 1, // 1 = Pendiente
                                    IdEstadoTributo = 6
                                };
                                _context.Tributos.Add(tributoFijo);
                            }
                        }

                        // Impuestos variables
                        var impuestosVariables = await _context.ContribuyentesImpuestosVariables
                            .Where(iv => iv.IdContribuyente == contribuyente.Id && iv.IdInmueble == inmueble.Id)
                            .ToListAsync();

                        foreach (var impuestoVar in impuestosVariables)
                        {
                            // Solo impuestos activos en fecha
                            if (impuestoVar.PeriodoDesde <= DateTime.Now && impuestoVar.PeriodoHasta >= DateTime.Now)
                            {
                                var valorVariable = await _context.ValoresTipoImpuesto
                                    .Where(v => v.TipoImpuestoId == impuestoVar.IdTipoImpuesto)
                                    .OrderByDescending(v => v.PeriodoDesde)
                                    .FirstOrDefaultAsync();

                                var tipoImpuestoVar = await _context.TiposImpuesto
                                    .FirstOrDefaultAsync(t => t.Id == impuestoVar.IdTipoImpuesto);

                                if (valorVariable != null && tipoImpuestoVar != null)
                                {
                                    bool existeTributoVar = await _context.Tributos.AnyAsync(t =>
                                        t.IdContribuyente == contribuyente.Id &&
                                        t.IdInmueble == inmueble.Id &&
                                        t.IdTipoImpuesto == impuestoVar.IdTipoImpuesto &&
                                        t.Periodo == periodoActual
                                    );

                                    if (!existeTributoVar)
                                    {
                                        var tributoVar = new Tributo
                                        {
                                            IdMunicipio = IdMunicipio,
                                            IdContribuyente = contribuyente.Id,
                                            IdInmueble = inmueble.Id,
                                            IdTipoImpuesto = impuestoVar.IdTipoImpuesto,
                                            Descripcion = tipoImpuestoVar.Descripcion,
                                            Periodo = periodoActual,
                                            Monto = valorVariable.Valor,
                                            FechaEmision = DateTime.Now,
                                            FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 10),
                                            IdEstado = 1,
                                            IdEstadoTributo = 6
                                        };
                                        _context.Tributos.Add(tributoVar);
                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
