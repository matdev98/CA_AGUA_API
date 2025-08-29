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

        public async Task<bool> UpdateEstadoIdAsync(int id, int idUsuario)
        {
            try
            {
                // Obtener el inmueble por su Id
                var pagos = await _context.Pagos.FindAsync(id);

                if (pagos == null)
                {
                    // El inmueble no fue encontrado
                    return false;
                }

                // Actualizar solo la propiedad EstadoId
                pagos.EstadoId = 2;
                pagos.OpAnula = idUsuario;
                pagos.Anulado = true;
                pagos.FecAnula = DateTime.Now;

                // Marcar la entidad como modificada y guardar los cambios
                _context.Pagos.Update(pagos);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw; 
            }
        }

        public async Task<List<Pago>> PagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            var existe = await _context.Contribuyentes.AnyAsync(c => c.Id == idContribuyente && c.EstadoId == 1);
            if (!existe) return new List<Pago>();

            var pagos = await _context.Pagos
                .Where(p => p.IdContribuyente == idContribuyente
                            && p.Idinmueble == idInmueble
                            && p.Periodo == periodo && p.EstadoId == 1)
                .ToListAsync();

            return pagos;
        }

        public async Task<Pago> GetByIdAsync(int id)
        {
            // Aquí buscas específicamente por IdPago
            return await _context.Pagos.FirstOrDefaultAsync(p => p.IdPago == id && p.EstadoId == 1);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pagoToDelete = await GetByIdAsync(id); // Llama a GetByIdAsync que ya busca por IdPago

            if (pagoToDelete == null)
            {
                return false; // No se encontró el pago
            }

            _context.Pagos.Remove(pagoToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio)
        {
            var pagosConMediosPago = await _context.Pagos
                .Where(p => p.FechaPago >= fechaInicio && p.FechaPago <= fechaFin && p.Idinmueble != null && p.EstadoId == 1)
                .Join(_context.MediosPago,
                    p => p.IdMedioPago,
                    mp => mp.Id,
                    (p, mp) => new { Pago = p, MedioPago = mp })
                .ToListAsync(); 

            
            var resultInMemory = pagosConMediosPago
                .GroupJoin(_context.Contribuyentes.AsEnumerable(), // Los contribuyentes se evalúan en memoria
                    combined => combined.Pago.IdContribuyente,
                    c => c.Id,
                    (combined, contribuyentes) => new { combined, Contribuyentes = contribuyentes.DefaultIfEmpty() })
                .SelectMany(combined => combined.Contribuyentes,
                    (combined, c) => new { combined.combined.Pago, combined.combined.MedioPago, Contribuyente = c })
                .GroupJoin(_context.Inmuebles.AsEnumerable(), // Los inmuebles se evalúan en memoria
                    combined => combined.Pago.Idinmueble,
                    i => i.Id,
                    (combined, inmuebles) => new { combined, Inmuebles = inmuebles.DefaultIfEmpty() })
                .SelectMany(combined => combined.Inmuebles,
                    (combined, i) => new { combined.combined.Pago, combined.combined.MedioPago, combined.combined.Contribuyente, Inmueble = i })
                .GroupJoin(_context.Localidades.AsEnumerable(), // Las localidades se evalúan en memoria
                    combined => combined.Inmueble != null ? combined.Inmueble.IdLocalidad : (int?)null, // Manejar potencial null en Inmueble
                    l => l.Id,
                    (combined, localidades) => new { combined, Localidades = localidades.DefaultIfEmpty() })
                .SelectMany(combined => combined.Localidades,
                    (combined, l) => new
                    {
                        Pago = combined.combined.Pago,
                        MedioPago = combined.combined.MedioPago,
                        Contribuyente = combined.combined.Contribuyente,
                        Inmueble = combined.combined.Inmueble,
                        Localidad = l
                    })
                // Aplicar el filtro de municipio en memoria (ya que Inmueble.IdMunicipio es accesible aquí)
                .Where(x => x.Inmueble != null && x.Inmueble.IdMunicipio == idMunicipio)
                .Select(x => new PagoDetalleDTO
                {
                    FechaPago = x.Pago.FechaPago,
                    NumeroDocumento = x.Contribuyente?.NumeroDocumento, // Usar null-conditional operator
                    Nombres = x.Contribuyente?.Nombres,
                    Apellidos = x.Contribuyente?.Apellidos,
                    IdInmueble = x.Pago.Idinmueble, // La FK directamente del Pago
                    Calle = x.Inmueble?.Calle,
                    Numero = x.Inmueble?.Numero,
                    Orientacion = x.Inmueble?.Orientacion,
                    Departamento = x.Localidad?.Departamento,
                    MedioPago = x.MedioPago?.Nombre,
                    MontoPagado = x.Pago.MontoPagado
                })
                .ToList(); // Materializa el resultado final en una lista

            return resultInMemory;
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
                    EstadoId = 1,
                    Monto = sumaMontosPagados,
                    Fecha = DateTime.Now // Fecha y hora actual por defecto
                };

                _context.CierreCaja.Add(nuevoCierre);
                await _context.SaveChangesAsync(); // Guarda el nuevo cierre para que se genere su ID

                foreach (var pago in pagosPendientesDeCierre)
                {
                    pago.IdCierre = nuevoCierre.Id; // Asignamos el ID del nuevo cierre a cada pago
                    pago.OpMod = idUsuario;
                    pago.FecMod = DateTime.Now;
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

        public async Task<bool> AnularCierreCajaAsync(int idCierre, int idUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cierreExistente = await _context.CierreCaja
                    .FirstOrDefaultAsync(c => c.Id == idCierre && c.EstadoId == 1);

                if (cierreExistente == null)
                {
                    await transaction.CommitAsync();
                    return false; // No se encontró el cierre de caja para anular
                }

                var pagosCierre = await _context.Pagos
                    .Where(p => p.IdCierre == idCierre)
                    .ToListAsync();

                if (!pagosCierre.Any())
                {
                    await transaction.CommitAsync();
                    return false; // No hay pagos asociados al cierre, no se puede anular
                }

                cierreExistente.Anulado = true; // Marca el cierre como anulado
                cierreExistente.EstadoId = 2; // Cambia el estado a anulado
                cierreExistente.OpAnula = idUsuario;
                cierreExistente.FecAnula = DateTime.Now;
                _context.CierreCaja.Update(cierreExistente); // Actualiza el cierre de caja

                foreach (var x in pagosCierre)
                {
                    x.IdCierre = 0; // Desvincula los pagos del cierre anulado
                    x.OpMod = idUsuario;
                    x.FecMod = DateTime.Now;
                    _context.Pagos.Update(x); // Actualiza cada pago
                }
                await _context.SaveChangesAsync(); // Guarda todos los cambios
                await transaction.CommitAsync(); // Confirma la transacción
                return true; // Anulación exitosa
            }
            catch 
            {
                await transaction.RollbackAsync(); // Revierte la transacción en caso de error
                throw new ApplicationException("Error al anular el cierre de caja."); // Lanza una excepción de aplicación
            }
        }

        public async Task<bool> Update(int id, Pago entidad)
        {
            try
            {
                var pago = await _context.Pagos.FindAsync(id);
                if (pago == null)
                {
                    return false; // No se encontró el pago
                }
                pago = entidad; // Actualiza todas las propiedades del pago

                _context.Pagos.Update(pago);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Pago> GetById(int id)
        {
            return await _context.Pagos.FirstOrDefaultAsync(p => p.IdPago == id);
        }
    }
}
