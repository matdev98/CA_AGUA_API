using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class CierreCajaRepository : BaseRepository<CierreCaja>, ICierreCajaRepository
    {
        private readonly AppDbContext _context;

        public CierreCajaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CierreCaja> RealizarCierreDeCajaAsync(int idUsuario, int idMunicipio)
        {
            // Usamos una transacción para asegurar la atomicidad de la operación
            // Si algo falla, todo se revierte.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var pagosPendientesDeCierre = await _context.Pagos
                    .Where(p => p.IdCierre == 0) 
                    .ToListAsync();

                if (!pagosPendientesDeCierre.Any())
                {
                    await transaction.CommitAsync(); // Confirmar la transacción aunque no haya cambios, o simplemente no iniciarla si se sabe de antemano.
                    return null; // No hay pagos para cerrar
                }

                var sumaMontosPagados = pagosPendientesDeCierre.Sum(p => p.MontoPagado ?? 0M); // Suma los montos, manejando nulos
                
                // 2. Agregar un registro nuevo en CierreCaja y que en el campo Monto se guarde la suma anterior.
                var nuevoCierre = new CierreCaja
                {
                    IdUsuario = idUsuario,
                    IdMunicipio = idMunicipio,
                    Monto = sumaMontosPagados,
                    Fecha = DateTime.Now,
                    EstadoId = 1
                };

                _context.CierreCaja.Add(nuevoCierre);
                await _context.SaveChangesAsync(); // Guarda el nuevo cierre para que se genere su ID

                foreach (var pago in pagosPendientesDeCierre)
                {
                    pago.IdCierre = nuevoCierre.Id; // Asignamos el ID del nuevo cierre a cada pago
                }

                await _context.SaveChangesAsync(); // Guarda los cambios en los pagos

                await transaction.CommitAsync(); // Confirma todos los cambios en la base de datos

                return nuevoCierre; // Devuelve el objeto CierreCaja recién creado
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // En caso de error, revierte la transacción

                throw new ApplicationException("Error al realizar el cierre de caja.", ex); // Lanza una excepción de aplicación
            }
        }

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> PagosCerrados (int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await (from p in _context.Pagos
                          join c in _context.Contribuyentes on p.IdContribuyente equals c.Id
                          join mp in _context.MediosPago on p.IdMedioPago equals mp.Id
                          join i in _context.Inmuebles on p.Idinmueble equals i.Id
                          where p.IdMunicipio == idMunicipio && p.IdCierre != 0 &&
                                p.FechaPago.HasValue &&
                                p.FechaPago.Value.Date >= fechaDesde.Date &&
                                p.FechaPago.Value.Date <= fechaHasta.Date &&
                                p.EstadoId == 1 // Solo pagos con EstadoId = 1 (asumiendo que significa "pagado")
                          select new PagoCerradoDetalleDTO
                          {
                              IdPago = p.IdPago,
                              FechaPago = p.FechaPago,
                              MontoPagado = p.MontoPagado,
                              Periodo = p.Periodo,
                              IdCierre = p.IdCierre,
                              IdMunicipio = p.IdMunicipio,

                              // Datos del Contribuyente
                              IdContribuyente = c.Id,
                              NombresContribuyente = c.Nombres,
                              ApellidosContribuyente = c.Apellidos,
                              NumeroDocumentoContribuyente = c.NumeroDocumento,

                              // Datos del Medio de Pago
                              IdMedioPago = mp.Id,
                              NombreMedioPago = mp.Nombre,

                              // Datos del Inmueble
                              IdInmueble = i.Id,
                              DireccionInmueble = i.Calle + " " + i.Numero + " " + i.Orientacion
                          })
                      .ToListAsync();
        }

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> PagosSinCerrar(int idMunicipio)
        {
            return await (from p in _context.Pagos
                          join c in _context.Contribuyentes on p.IdContribuyente equals c.Id
                          join mp in _context.MediosPago on p.IdMedioPago equals mp.Id
                          join i in _context.Inmuebles on p.Idinmueble equals i.Id
                          where p.IdMunicipio == idMunicipio && p.IdCierre == 0 && p.EstadoId == 1
                          select new PagoCerradoDetalleDTO
                          {
                              IdPago = p.IdPago,
                              FechaPago = p.FechaPago,
                              MontoPagado = p.MontoPagado,
                              Periodo = p.Periodo,
                              IdCierre = p.IdCierre,
                              IdMunicipio = p.IdMunicipio,

                              // Datos del Contribuyente
                              IdContribuyente = c.Id,
                              NombresContribuyente = c.Nombres,
                              ApellidosContribuyente = c.Apellidos,
                              NumeroDocumentoContribuyente = c.NumeroDocumento,

                              // Datos del Medio de Pago
                              IdMedioPago = mp.Id,
                              NombreMedioPago = mp.Nombre,

                              // Datos del Inmueble
                              IdInmueble = i.Id,
                              DireccionInmueble = i.Calle + " " + i.Numero + " " + i.Orientacion
                          }
                          )
                .ToListAsync();
        }

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> PagosdeunCierre (int idMunicipio, int idCierre)
        {
            return await (from p in _context.Pagos
                         join c in _context.Contribuyentes on p.IdContribuyente equals c.Id
                         join mp in _context.MediosPago on p.IdMedioPago equals mp.Id
                         join i in _context.Inmuebles on p.Idinmueble equals i.Id
                         where p.IdMunicipio == idMunicipio && p.IdCierre == idCierre && p.EstadoId == 1
                         select new PagoCerradoDetalleDTO
                         {
                             IdPago = p.IdPago,
                             FechaPago = p.FechaPago,
                             MontoPagado = p.MontoPagado,
                             Periodo = p.Periodo,
                             IdCierre = p.IdCierre,
                             IdMunicipio = p.IdMunicipio,

                             // Datos del Contribuyente
                             IdContribuyente = c.Id,
                             NombresContribuyente = c.Nombres,
                             ApellidosContribuyente = c.Apellidos,
                             NumeroDocumentoContribuyente = c.NumeroDocumento,

                             // Datos del Medio de Pago
                             IdMedioPago = mp.Id,
                             NombreMedioPago = mp.Nombre,

                             // Datos del Inmueble
                             IdInmueble = i.Id,
                             DireccionInmueble = i.Calle + " " + i.Numero + " " + i.Orientacion
                         })
                .ToListAsync();
        }

        public async Task<IEnumerable<CierreCaja>> GetCierreCajaPeriodo(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _context.CierreCaja
                .Where(cc => cc.IdMunicipio == idMunicipio && cc.EstadoId == 1 && cc.Fecha.Date >= fechaDesde.Date && cc.Fecha.Date <= fechaHasta.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateEstadoIdAsync(int id)
        {
            try
            {
                // Obtener el contribuyente por su Id
                var cierreCaja = await _context.CierreCaja.FindAsync(id);

                if (cierreCaja == null)
                {
                    // El contribuyente no fue encontrado
                    return false;
                }

                // Actualizar solo la propiedad EstadoId
                cierreCaja.EstadoId = 2;

                // Marcar la entidad como modificada y guardar los cambios
                _context.CierreCaja.Update(cierreCaja); // O _context.Entry(contribuyente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw; // Relanza la excepción para que el servicio la capture
            }
        }

        public async Task<string> GetMunicipio(int idMunicipio)
        {
            return await _context.Municipios
                .Where(m => m.Id == idMunicipio)
                .Select(m => m.Nombre)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        public async Task<string> GetFechaCierre(int idCierre) 
        {
            var fechaCierre = await _context.CierreCaja
                .Where(cc => cc.Id == idCierre)
                .Select(cc => cc.Fecha)
                .FirstOrDefaultAsync();
            return fechaCierre.ToString("dd/MM/yyyy");
        }

        public async Task<string> GetLogoMunicipio(int idMunicipio)
        {
            return await _context.Municipios
                .Where(m => m.Id == idMunicipio)
                .Select(m => m.Logo)
                .FirstOrDefaultAsync() ?? "";
        }
    }
}
