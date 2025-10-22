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
    public class ImpuestosVariablesRepository : BaseRepository<ContribuyentesImpuestosVariables>, IImpuestosVariablesRepository
    {
        private readonly AppDbContext _context;

        public ImpuestosVariablesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TributoContribuyenteDTO>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            var result = new List<TributoContribuyenteDTO>();

            var contribuyente = await _context.Contribuyentes
                .FirstOrDefaultAsync(c => c.Id == contribuyenteId);

            if (contribuyente == null)
                return result;

            var inmuebles = await _context.Inmuebles
                .Where(i => i.IdContribuyente == contribuyenteId)
                .ToListAsync();

            if (!inmuebles.Any())
                return result;

            // Suponiendo que el impuesto fijo siempre tiene un ID específico (por ejemplo, 1)
            int tipoImpuestoFijoId = 1;

            var valorImpuesto = await _context.ValoresTipoImpuesto
                .Where(v => v.TipoImpuestoId == tipoImpuestoFijoId)
                .OrderByDescending(v => v.PeriodoDesde)
                .FirstOrDefaultAsync();

            if (valorImpuesto == null)
                return result;

            var tipoImpuesto = await _context.TiposImpuesto
                .FirstOrDefaultAsync(t => t.Id == tipoImpuestoFijoId);

            if (tipoImpuesto == null)
                return result;



            foreach (var inmueble in inmuebles)
            {

                var tributoFijo = await _context.Tributos
                    .FirstOrDefaultAsync(t => t.IdInmueble == inmueble.Id && t.IdTipoImpuesto == tipoImpuesto.Id);

                var pagoFijo = tributoFijo != null
                    ? await _context.Pagos.FirstOrDefaultAsync(p => p.IdTributo == tributoFijo.Id)
                    : null;

                var estadoFijo = pagoFijo != null && pagoFijo.MontoPagado.HasValue && pagoFijo.MontoPagado.Value >= tributoFijo.Monto ? "Pagada" :
                                 pagoFijo != null && pagoFijo.MontoPagado.HasValue && pagoFijo.MontoPagado.Value < tributoFijo.Monto && tributoFijo.FechaVencimiento < DateTime.Now ? "Parcial/Vencida" :
                                 pagoFijo != null && pagoFijo.MontoPagado.HasValue && pagoFijo.MontoPagado.Value < tributoFijo.Monto ? "Parcial" :
                                 pagoFijo == null && tributoFijo.FechaVencimiento < DateTime.Now ? "Vencida" :
                                 "Pendiente";


                result.Add(new TributoContribuyenteDTO
                {
                    Id = 0,
                    IdMunicipio = tributoFijo.IdMunicipio, // o donde corresponda
                    IdContribuyente = contribuyenteId,
                    Nombre = contribuyente.Nombres.Trim(),
                    Apellido = contribuyente.Apellidos.Trim(),
                    Direccion = $"{inmueble.Calle} {inmueble.Numero} {inmueble.Orientacion}",
                    Documento = contribuyente.NumeroDocumento.Trim(),
                    IdInmueble = inmueble.Id,
                    IdTipoImpuesto = tipoImpuesto.Id,
                    Descripcion = tipoImpuesto.Descripcion.Trim(),
                    Periodo = $"{valorImpuesto.PeriodoDesde:MM/yyyy} - {valorImpuesto.PeriodoHasta:MM/yyyy}",
                    Monto = inmueble.AreaTotal * valorImpuesto.Valor,
                    FechaEmision = tributoFijo.FechaEmision,
                    FechaVencimiento = tributoFijo.FechaVencimiento,
                    IdEstado = tributoFijo.IdEstado,
                    EstadoTributoDescripcion = estadoFijo
                });
            }
            // Impuestos variables
            var inmuebleimpuestosVariables = await _context.ContribuyentesImpuestosVariables
                .Where(v => v.IdContribuyente == contribuyenteId)
                .ToListAsync();

            foreach (var impuesto in inmuebleimpuestosVariables)
            {
                var valorVariable = await _context.ValoresTipoImpuesto
                    .Where(v => v.TipoImpuestoId == impuesto.IdTipoImpuesto)
                    .OrderByDescending(v => v.PeriodoDesde)
                    .FirstOrDefaultAsync();

                var tipoImpuestoVar = await _context.TiposImpuesto
                    .FirstOrDefaultAsync(t => t.Id == impuesto.IdTipoImpuesto);

                // Verificamos si el tipo de impuesto existe
                if (tipoImpuestoVar == null) continue;

                // Obtener el inmueble asociado al impuesto
                var inmueble = inmuebles.FirstOrDefault(i => i.Id == impuesto.IdInmueble);

                // Si el inmueble no se encuentra en la lista, continuamos con el siguiente impuesto
                if (inmueble == null) continue;

                // Obtener el tributo correspondiente a este impuesto variable
                var tributoVar = await _context.Tributos
                    .FirstOrDefaultAsync(t => t.IdInmueble == inmueble.Id && t.IdTipoImpuesto == tipoImpuestoVar.Id);

                // Si no existe el tributo, continuamos con el siguiente impuesto
                if (tributoVar == null) continue;

                // Verificar si el periodo de impuesto variable está dentro del rango
                if (valorVariable != null && tipoImpuestoVar != null)
                {
                    // Verificamos si el impuesto variable está dentro del rango de fechas
                    if (impuesto.PeriodoDesde <= DateTime.Now && impuesto.PeriodoHasta >= DateTime.Now)
                    {


                        // Obtener el pago asociado al tributo
                        var pagoVariable = await _context.Pagos
                            .FirstOrDefaultAsync(p => p.IdTributo == tributoVar.Id);

                        // Calcular el estado del tributo (Pagada, Parcial, Vencida, etc.)
                        var estadoVariable = pagoVariable != null && pagoVariable.MontoPagado.HasValue && pagoVariable.MontoPagado.Value >= tributoVar.Monto ? "Pagada" :
                                             pagoVariable != null && pagoVariable.MontoPagado.HasValue && pagoVariable.MontoPagado.Value < tributoVar.Monto && tributoVar.FechaVencimiento < DateTime.Now ? "Parcial/Vencida" :
                                             pagoVariable != null && pagoVariable.MontoPagado.HasValue && pagoVariable.MontoPagado.Value < tributoVar.Monto ? "Parcial" :
                                             pagoVariable == null && tributoVar.FechaVencimiento < DateTime.Now ? "Vencida" :
                                             "Pendiente";

                        // Agregar el tributo variable al resultado
                        result.Add(new TributoContribuyenteDTO
                        {
                            Id = 0,
                            IdMunicipio = tributoVar.IdMunicipio,
                            IdContribuyente = contribuyenteId,
                            Nombre = contribuyente.Nombres.Trim(),
                            Apellido = contribuyente.Apellidos.Trim(),
                            Direccion = $"{inmueble.Calle} {inmueble.Numero} {inmueble.Orientacion}",
                            Documento = contribuyente.NumeroDocumento.Trim(),
                            IdInmueble = inmueble.Id,
                            IdTipoImpuesto = tipoImpuestoVar.Id,
                            Descripcion = tipoImpuestoVar.Descripcion.Trim(),
                            Periodo = $"{impuesto.PeriodoDesde:MM/yyyy} - {impuesto.PeriodoHasta:MM/yyyy}",
                            Monto = valorVariable.Valor,
                            FechaEmision = tributoVar.FechaEmision,
                            FechaVencimiento = tributoVar.FechaVencimiento,
                            IdEstado = tributoVar.IdEstado,
                            EstadoTributoDescripcion = estadoVariable
                        });
                    }
                }
            }

            return result.OrderBy(r => r.IdInmueble).ToList();


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
                                    FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month+1, 10),
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
                                            FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month+1, 10),
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
    }
}
