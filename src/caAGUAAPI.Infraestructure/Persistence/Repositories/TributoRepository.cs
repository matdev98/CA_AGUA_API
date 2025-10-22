using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using caAGUAAPI.Infraestructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Infraestructure.Persistence.Repositories
{
    public class TributoRepository : BaseRepository<Tributo>, ITributoRepository
    {
        private readonly AppDbContext _context;
        private readonly IBaseService<ContribuyentesImpuestosVariables> _baseService;
        private readonly ILogger<ContribuyentesImpuestosVariables> _logger;

        public TributoRepository(IBaseService<ContribuyentesImpuestosVariables> baseService, ILogger<ContribuyentesImpuestosVariables> logger, AppDbContext context) : base(context)
        {
            _context = context;
            _baseService = baseService;
            _logger = logger;
        }

        public async Task<IEnumerable<TributoContribuyenteDTO>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            var hoy = DateTime.Today;
            var tributos = new List<TributoContribuyenteDTO>();

            // 1. Obtener el contribuyente
            var contribuyente = await _context.Contribuyentes.FindAsync(contribuyenteId);
            if (contribuyente == null)
            {
                return tributos; // Devolver lista vacía si no se encuentra
            }

            // 2. Obtener los inmuebles activos asociados a este contribuyente
            // Ahora, los inmuebles se obtienen directamente de la tabla Inmuebles, filtrando por IdContribuyente.
            var inmuebles = await _context.Inmuebles
                .Where(i => i.IdContribuyente == contribuyenteId && i.EstadoId == 1)
                .ToListAsync();

            if (!inmuebles.Any())
            {
                return tributos; // Devolver lista vacía si no hay inmuebles
            }

            // 3. Obtener todos los impuestos variables relevantes en una sola consulta
            // Esta consulta ahora unirá ContribuyentesImpuestosVariables con TiposImpuesto y ValoresTipoImpuesto,
            // y también con Inmuebles para obtener sus detalles.
            var tributosVariablesQuery = from civ in _context.ContribuyentesImpuestosVariables
                                         join ti in _context.TiposImpuesto on civ.IdTipoImpuesto equals ti.Id
                                         join vti in _context.ValoresTipoImpuesto on civ.IdTipoImpuesto equals vti.TipoImpuestoId
                                         join inm in _context.Inmuebles on civ.IdInmueble equals inm.Id // Unimos con Inmuebles aquí
                                         where civ.IdContribuyente == contribuyenteId
                                               && ti.EstadoId == 1
                                               && hoy >= civ.PeriodoDesde
                                               && hoy <= civ.PeriodoHasta
                                               && inm.EstadoId == 1 // Aseguramos que el inmueble también esté activo
                                         select new { civ, ti, vti, inm }; // Seleccionamos también el inmueble

            var tributosVariablesRaw = await tributosVariablesQuery.ToListAsync();

            // 4. Crear la lista final de DTOs
            foreach (var rawData in tributosVariablesRaw)
            {
                // Ya tenemos el inmueble dentro de rawData, no necesitamos buscarlo de nuevo
                var inmueble = rawData.inm;

                DateTime fechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(9);

                tributos.Add(new TributoContribuyenteDTO
                {
                    Id = rawData.civ.Id,
                    IdMunicipio = rawData.ti.MunicipioId,
                    IdContribuyente = contribuyente.Id,
                    Nombre = contribuyente.Nombres.Trim(),
                    Apellido = contribuyente.Apellidos.Trim(),
                    Direccion = $"{inmueble.Calle} {inmueble.Numero}",
                    Documento = contribuyente.NumeroDocumento.Trim(),
                    IdInmueble = inmueble.Id,
                    IdTipoImpuesto = rawData.ti.Id,
                    Descripcion = rawData.ti.Descripcion.Trim(),
                    Periodo = $"{rawData.civ.PeriodoDesde:yyyy-MM-dd} al {rawData.civ.PeriodoHasta:yyyy-MM-dd}",
                    Monto = rawData.vti.Valor,
                    FechaEmision = hoy,
                    FechaVencimiento = fechaVencimiento,
                    IdEstado = 1,
                    EstadoTributoDescripcion = "Pendiente"
                });
            }

            return tributos;
        }

        //public async Task<IEnumerable<TributoContribuyenteDTO>> GetByContribuyenteIdAsync(int contribuyenteId)
        //{
        //    var hoy = DateTime.Today;
        //    var tributos = new List<TributoContribuyenteDTO>();

        //    var contribuyente = await _context.Contribuyentes.FindAsync(contribuyenteId);
        //    if (contribuyente == null) return tributos;

        //    var inmuebles = await _context.Inmuebles
        //        .Where(i => i.IdContribuyente == contribuyenteId && i.EstadoId == 1)
        //        .ToListAsync();

        //    var tipoImpuestoFijo = await _context.TiposImpuesto.FirstOrDefaultAsync(t => t.Id == 1);
        //    var valorFijo = await _context.ValoresTipoImpuesto.FirstOrDefaultAsync(v => v.TipoImpuestoId == 1);

        //    foreach (var inmueble in inmuebles)
        //    {
        //        // Siempre agregar impuesto fijo
        //        if (tipoImpuestoFijo != null && valorFijo != null)
        //        {
        //            decimal montoFijo = inmueble.AreaTotal * valorFijo.Valor;
        //            DateTime fechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(9);

        //            tributos.Add(new TributoContribuyenteDTO
        //            {
        //                Id = 0,
        //                IdMunicipio = tipoImpuestoFijo.MunicipioId,
        //                IdContribuyente = contribuyente.Id,
        //                Nombre = contribuyente.Nombres.Trim(),
        //                Apellido = contribuyente.Apellidos.Trim(),
        //                Direccion = $"{inmueble.Calle} {inmueble.Numero}",
        //                Documento = contribuyente.NumeroDocumento.Trim(),
        //                IdInmueble = inmueble.Id,
        //                IdTipoImpuesto = tipoImpuestoFijo.Id,
        //                Descripcion = tipoImpuestoFijo.Descripcion.Trim(),
        //                Periodo = "",
        //                Monto = montoFijo,
        //                FechaEmision = hoy,
        //                FechaVencimiento = fechaVencimiento,
        //                IdEstado = 1,
        //                EstadoTributoDescripcion = "Pendiente"
        //            });
        //        }

        //        // Agregar todos los impuestos variables vigentes
        //        var impuestosVariables = await _context.ContribuyentesImpuestosVariables
        //            .Where(iv => iv.IdContribuyente == contribuyenteId
        //                && iv.IdInmueble == inmueble.Id
        //                && hoy >= iv.PeriodoDesde
        //                && hoy <= iv.PeriodoHasta)
        //            .ToListAsync();

        //        foreach (var variable in impuestosVariables)
        //        {
        //            var tipoImpuesto = await _context.TiposImpuesto
        //                .FirstOrDefaultAsync(t => t.Id == variable.IdTipoImpuesto && t.EstadoId == 1);

        //            if (tipoImpuesto == null)
        //                continue;

        //            // Buscamos el valor correspondiente al tipo de impuesto
        //            var valorVariable = await _context.ValoresTipoImpuesto
        //                .FirstOrDefaultAsync(v => v.TipoImpuestoId == variable.IdTipoImpuesto);

        //            if (valorVariable == null)
        //                continue;

        //            decimal monto = valorVariable.Valor;
        //            DateTime fechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(9);

        //            tributos.Add(new TributoContribuyenteDTO
        //            {
        //                Id = variable.Id,
        //                IdMunicipio = tipoImpuesto.MunicipioId,
        //                IdContribuyente = contribuyente.Id,
        //                Nombre = contribuyente.Nombres.Trim(),
        //                Apellido = contribuyente.Apellidos.Trim(),
        //                Direccion = $"{inmueble.Calle} {inmueble.Numero}",
        //                Documento = contribuyente.NumeroDocumento.Trim(),
        //                IdInmueble = inmueble.Id,
        //                IdTipoImpuesto = tipoImpuesto.Id,
        //                Descripcion = tipoImpuesto.Descripcion.Trim(),
        //                Periodo = $"{variable.PeriodoDesde:yyyy-MM-dd} al {variable.PeriodoHasta:yyyy-MM-dd}",
        //                Monto = monto,
        //                FechaEmision = hoy,
        //                FechaVencimiento = fechaVencimiento,
        //                IdEstado = 1,
        //                EstadoTributoDescripcion = "Pendiente"
        //            });
        //        }

        //    }

        //    return tributos;
        //}

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
                                   where tributo.Periodo == periodo && tributo.IdMunicipio == idMunicipio && tributo.IdEstado == 1
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
                .FirstOrDefaultAsync(c => c.Id == idContribuyente && c.EstadoId == 1);

            if (contribuyente == null)
                return new List<TributoAgrupadoDTO>();

            var tributos = await _context.Tributos
                .Where(t => t.IdContribuyente == idContribuyente && t.Periodo == periodo && t.IdEstado == 1)
                .ToListAsync();

            var inmueblesIds = tributos.Select(t => t.IdInmueble).Distinct().ToList();

            var inmuebles = await _context.Inmuebles
                .Where(i => inmueblesIds.Contains(i.Id) && i.EstadoId == 1)
                .ToDictionaryAsync(i => i.Id, i => i);

            var pagosSet = await _context.Pagos
                .Where(p => p.Periodo == periodo && p.EstadoId == 1 && inmueblesIds.Contains(p.Idinmueble))
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
                    var totalTributoInmueble = g.Sum(t => t.Monto);

                    var sumaPagosInmueble = pagosSet
                        .Where(p => p.Idinmueble == g.Key)
                        .Sum(p => p.MontoPagado);

                    bool estaPagadoCompletamente = sumaPagosInmueble >= totalTributoInmueble;
                    bool estaPagadoParcialmente = sumaPagosInmueble > 0 && !estaPagadoCompletamente;
                    bool estaVencido = primerTributo.FechaVencimiento < DateTime.Now;

                    int estadoId;
                    string estadoBaseDescripcion;
                    string estadoDescripcionFinal;

                    if (estaPagadoCompletamente)
                    {
                        estadoId = (int)EstadoTributoEnum.Pagada;
                    }
                    else if (estaPagadoParcialmente && estaVencido)
                    {
                        estadoId = (int)EstadoTributoEnum.ParcialmentePagadoVencido;
                    }
                    else if (estaPagadoParcialmente)
                    {
                        estadoId = (int)EstadoTributoEnum.ParcialmentePagado;
                    }
                    else if (estaVencido)
                    {
                        estadoId = (int)EstadoTributoEnum.VencidoSinPago;
                    }
                    else
                    {
                        estadoId = (int)EstadoTributoEnum.Pendiente;
                    }

                    estadoBaseDescripcion = estadosTributo.TryGetValue(estadoId, out var desc)
                        ? desc
                        : "Desconocido";

                    
                    if (estaPagadoParcialmente && !estaVencido)
                    {
                        estadoDescripcionFinal = $"Pagada parcialmente";
                    }
                    else if (estaPagadoParcialmente && estaVencido)
                    {
                        estadoDescripcionFinal = $"Pagada parcialmente y vencida";
                    }
                    else if (!estaPagadoCompletamente && estaVencido && !estaPagadoParcialmente)
                    {
                        estadoDescripcionFinal = $"Vencida";
                    }
                    else
                    {
                        estadoDescripcionFinal = estadoBaseDescripcion;
                    }

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
                        FechaVencimiento = primerTributo.FechaVencimiento,
                        Monto = totalTributoInmueble,
                        IdEstado = estadoId,
                        EstadoTributoDescripcion = estadoDescripcionFinal
                    };
                }).ToList();

            return agrupados;
        }

        public async Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupadosSinPeriodo(int idContribuyente)
        {
            // Obtener el contribuyente
            var contribuyente = await _context.Contribuyentes
                .FirstOrDefaultAsync(c => c.Id == idContribuyente && c.EstadoId == 1);

            if (contribuyente == null)
            {
                return new List<TributoAgrupadoDTO>();
            }

            // Traer TODOS los tributos del contribuyente
            // y luego los ordenamos para agrupar por el más nuevo.
            var tributos = await _context.Tributos
                .Where(t => t.IdContribuyente == idContribuyente && t.IdEstado == 1) 
                .OrderByDescending(t => t.FechaEmision)
                .ToListAsync();

            if (!tributos.Any())
            {
                return new List<TributoAgrupadoDTO>();
            }

            // Obtenemos los IDs de los inmuebles involucrados para cargarlos eficientemente
            var inmueblesIds = tributos.Select(t => t.IdInmueble).Distinct().ToList();

            var inmuebles = await _context.Inmuebles
                .Where(i => inmueblesIds.Contains(i.Id) && i.EstadoId == 1)
                .ToDictionaryAsync(i => i.Id, i => i);

            // Obtener todos los pagos relevantes para los inmuebles y cualquier período
            var pagosSet = await _context.Pagos
                .Where(p => p.EstadoId == 1 && inmueblesIds.Contains(p.Idinmueble))
                .ToListAsync();

            // Obtener descripciones desde la tabla EstadoTributo
            var estadosTributo = await _context.EstadoTributos
                .ToDictionaryAsync(e => e.Id, e => e.Descripcion); // Cargar todos los estados para flexibilidad

            var agrupados = new List<TributoAgrupadoDTO>();

            // Agrupamos los tributos por inmueble y período para procesarlos
            // El orden descendente inicial de 'tributos' ayudará a que el 'First()' sea el más nuevo de cada grupo de inmueble+periodo.
            var gruposPorInmuebleYPeriodo = tributos
                .GroupBy(t => new { t.IdInmueble, t.Periodo }) // Agrupamos por Inmueble Y Período
                .OrderByDescending(g => g.Key.Periodo) // Ordenamos los grupos por período descendente
                .ToList();

            foreach (var grupo in gruposPorInmuebleYPeriodo)
            {
                var primerTributoEnGrupo = grupo.First(); // Este será el tributo más reciente para este inmueble y período
                var inmueble = inmuebles.GetValueOrDefault(grupo.Key.IdInmueble);

                if (inmueble == null)
                {
                    continue; // Si el inmueble no es válido, saltamos este grupo
                }

                var totalTributoInmueblePeriodo = grupo.Sum(t => t.Monto);

                // Sumar pagos para este inmueble y este período
                var sumaPagosInmueblePeriodo = pagosSet
                    .Where(p => p.Idinmueble == grupo.Key.IdInmueble && p.Periodo == grupo.Key.Periodo)
                    .Sum(p => p.MontoPagado);

                bool estaPagadoCompletamente = sumaPagosInmueblePeriodo >= totalTributoInmueblePeriodo;
                bool estaPagadoParcialmente = sumaPagosInmueblePeriodo > 0 && !estaPagadoCompletamente;
                bool estaVencido = primerTributoEnGrupo.FechaVencimiento < DateTime.Now;

                int estadoId;
                string estadoBaseDescripcion;
                string estadoDescripcionFinal;

                // Lógica para determinar el estado (sin cambios significativos aquí)
                if (estaPagadoCompletamente)
                {
                    estadoId = (int)EstadoTributoEnum.Pagada;
                }
                else if (estaPagadoParcialmente && estaVencido)
                {
                    estadoId = (int)EstadoTributoEnum.ParcialmentePagadoVencido;
                }
                else if (estaPagadoParcialmente)
                {
                    estadoId = (int)EstadoTributoEnum.ParcialmentePagado;
                }
                else if (estaVencido)
                {
                    estadoId = (int)EstadoTributoEnum.VencidoSinPago;
                }
                else
                {
                    estadoId = (int)EstadoTributoEnum.Pendiente;
                }

                estadoBaseDescripcion = estadosTributo.TryGetValue(estadoId, out var desc)
                    ? desc
                    : "Desconocido";

                if (estaPagadoParcialmente && !estaVencido)
                {
                    estadoDescripcionFinal = $"Pagada parcialmente";
                }
                else if (estaPagadoParcialmente && estaVencido)
                {
                    estadoDescripcionFinal = $"Pagada parcialmente y vencida";
                }
                else if (!estaPagadoCompletamente && estaVencido && !estaPagadoParcialmente)
                {
                    estadoDescripcionFinal = $"Vencida";
                }
                else
                {
                    estadoDescripcionFinal = estadoBaseDescripcion;
                }

                agrupados.Add(new TributoAgrupadoDTO
                {
                    IdMunicipio = primerTributoEnGrupo.IdMunicipio,
                    IdContribuyente = primerTributoEnGrupo.IdContribuyente,
                    Nombre = contribuyente.Nombres,
                    Apellido = contribuyente.Apellidos,
                    Direccion = inmueble != null ? $"{inmueble.Calle} {inmueble.Numero}" : "Dirección no disponible", // Manejo de nulos
                    Documento = contribuyente.NumeroDocumento,
                    IdInmueble = grupo.Key.IdInmueble,
                    Periodo = grupo.Key.Periodo, // Usar el período del grupo
                    FechaVencimiento = primerTributoEnGrupo.FechaVencimiento,
                    Monto = totalTributoInmueblePeriodo,
                    IdEstado = estadoId,
                    EstadoTributoDescripcion = estadoDescripcionFinal
                });
            }

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
                      && tributo.Periodo == periodo && tributo.IdEstado == 1
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

        public async Task<List<TributoContribuyenteDTO>> ObtenerTodosLosTributosDelContribuyentePorPeriodo(int idContribuyente, string periodo)
        {
            var contribuyente = await GetByIdAsync(idContribuyente);
            if (contribuyente == null || contribuyente.IdEstado != 1) 
            {
                return new List<TributoContribuyenteDTO>();
            }

            var inmueblesAgrupados = await ObtenerTributosAgrupados(idContribuyente, periodo);
            var inmueblesfiltrados = inmueblesAgrupados.Where(c => c.IdEstado == 1);
            if (inmueblesfiltrados == null || !inmueblesfiltrados.Any()) return new List<TributoContribuyenteDTO>();

            var todosLosTributos = new List<TributoContribuyenteDTO>();
            foreach (var inmueble in inmueblesAgrupados)
            {
                var detalles = await ObtenerDetalleTributoPorInmuebleAsync(idContribuyente, inmueble.IdInmueble, periodo);
                todosLosTributos.AddRange(detalles);
            }
            return todosLosTributos;
        }

        public async Task GenerarTributosDelMesAsync(int IdMunicipio, int idUsuario)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var periodoActual = DateTime.Now.ToString("yyyyMM");

                // Traemos todos los inmuebles que tengan contribuyente asignado y estén activos
                var inmuebles = await _context.Inmuebles
                    .Where(i => i.IdMunicipio == IdMunicipio && i.IdContribuyente != null && i.EstadoId == 1)
                    .ToListAsync();

                foreach (var inmueble in inmuebles)
                {
                    int idContribuyente = inmueble.IdContribuyente;

                    // ========================
                    // Generar tributos variables
                    // ========================
                    var impuestosVariables = await _context.ContribuyentesImpuestosVariables
                        .Where(iv => iv.IdContribuyente == idContribuyente && iv.IdInmueble == inmueble.Id)
                        .ToListAsync();

                    foreach (var impuestoVar in impuestosVariables)
                    {
                        // Verifica si el impuesto variable está vigente en el período actual
                        if (impuestoVar.PeriodoDesde <= DateTime.Now && impuestoVar.PeriodoHasta >= DateTime.Now)
                        {
                            var valorVariable = await _context.ValoresTipoImpuesto
                                .Where(v => v.TipoImpuestoId == impuestoVar.IdTipoImpuesto)
                                .OrderByDescending(v => v.PeriodoDesde) // Asegura obtener el valor más reciente
                                .FirstOrDefaultAsync();

                            var tipoImpuestoVar = await _context.TiposImpuesto
                                .FirstOrDefaultAsync(t => t.Id == impuestoVar.IdTipoImpuesto && t.EstadoId == 1); // Asegura que el tipo de impuesto esté activo

                            // Verifica si el tributo variable para este inmueble, tipo de impuesto y período ya existe
                            bool existeTributoVar = await _context.Tributos.AnyAsync(t =>
                                t.IdContribuyente == idContribuyente &&
                                t.IdInmueble == inmueble.Id &&
                                t.IdTipoImpuesto == impuestoVar.IdTipoImpuesto &&
                                t.Periodo == periodoActual &&
                                t.IdEstado == 1); // Asume que IdEstado = 1 significa un tributo pendiente/activo no pagado para este período

                            if (valorVariable != null && tipoImpuestoVar != null && !existeTributoVar)
                            {
                                var tributoVar = new Tributo
                                {
                                    IdMunicipio = IdMunicipio,
                                    IdContribuyente = idContribuyente,
                                    IdInmueble = inmueble.Id,
                                    IdTipoImpuesto = impuestoVar.IdTipoImpuesto,
                                    Descripcion = tipoImpuestoVar.Descripcion,
                                    Periodo = periodoActual,
                                    Monto = valorVariable.Valor,
                                    FechaEmision = DateTime.Now,
                                    FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 10), // Vence el 10 del mes siguiente
                                    IdEstado = 1,      // Estado del tributo (ej. "Pendiente de pago")
                                    IdEstadoTributo = 6, // Estado específico de tributo (ej. "Generado")
                                    OpCrea = idUsuario,
                                    FecCrea = DateTime.Now
                                };
                                _context.Tributos.Add(tributoVar);
                            }
                        }
                    }
                }

                //Aca SP de Codigo de Barra


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();




            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Error al generar tributos variables", ex); // Lanzar una excepción de aplicación más clara
            }
        }

        public async Task<List<TipoImpuesto>> GetFijos(int idMunicipio)
        {
            var tributosFijos = await _context.TiposImpuesto
                .Where(t => t.MunicipioId == idMunicipio && t.EstadoId == 1 && t.CobroFijo) // Aseguramos que sea el tipo de impuesto fijo
                .Select(t => new TipoImpuesto
                {
                    Id = t.Id,
                    Descripcion = t.Descripcion,
                    MunicipioId = t.MunicipioId,
                    CobroFijo = t.CobroFijo,
                    EstadoId = t.EstadoId
                })
                .ToListAsync();

            return tributosFijos;
        }

        public async Task<bool> ApplyToAll(TipoImpuesto impuesto, int idMunicipio)
        {
            try
            {
                var ahora = DateTime.Now;
                var hasta = ahora.AddYears(1);

                var inmuebles = await _context.Inmuebles
                    .Where(i => i.IdMunicipio == idMunicipio && i.EstadoId == 1)
                    .ToListAsync();

                if (!inmuebles.Any())
                    return true;

                var inmuebleIds = inmuebles.Select(i => i.Id).ToList();

                var existentes = await _context.ContribuyentesImpuestosVariables
                    .Where(c =>
                        inmuebleIds.Contains(c.IdInmueble) &&
                        c.IdTipoImpuesto == impuesto.Id &&
                        c.PeriodoDesde <= ahora &&
                        c.PeriodoHasta >= ahora)
                    .ToListAsync();

                var existentesPorInmueble = existentes.ToLookup(e => e.IdInmueble);

                foreach (var i in inmuebles)
                {
                    if (!existentesPorInmueble.Contains(i.Id))
                    {
                        var nuevoCobro = new ContribuyentesImpuestosVariables
                        {
                            IdContribuyente = i.IdContribuyente,
                            IdTipoImpuesto = impuesto.Id,
                            IdInmueble = i.Id,
                            PeriodoDesde = ahora,
                            PeriodoHasta = hasta
                        };
                        await _baseService.AddAsync(nuevoCobro);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aplicar impuesto a todos los inmuebles del municipio "+ idMunicipio, idMunicipio);
                return false;
            }
        }

    }
}
