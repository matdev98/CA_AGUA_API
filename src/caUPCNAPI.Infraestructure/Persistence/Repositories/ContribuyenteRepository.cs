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
    public class ContribuyenteRepository : BaseRepository<Contribuyente>, IContribuyenteRepository
    {
        private readonly AppDbContext _context;

        public ContribuyenteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _context.Contribuyentes
                .Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1)
                .ToListAsync();
        }

        public async Task<bool> UpdateEstadoIdAsync(int id, int idUsuario)
        {
            try
            {
                // Obtener el contribuyente por su Id
                var contribuyente = await _context.Contribuyentes.FindAsync(id);

                if (contribuyente == null)
                {
                    // El contribuyente no fue encontrado
                    return false;
                }

                // Actualizar solo la propiedad EstadoId
                contribuyente.EstadoId = 2;
                contribuyente.FecAnula = DateTime.Now;
                contribuyente.OpAnula = idUsuario;
                contribuyente.Anulado = true;

                // Marcar la entidad como modificada y guardar los cambios
                _context.Contribuyentes.Update(contribuyente); // O _context.Entry(contribuyente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw; // Relanza la excepción para que el servicio la capture
            }
        }

        /// <summary>
        /// Obtiene una lista de los últimos 10 contribuyentes agregados,
        /// ordenados por ID de forma descendente y tomando los primeros 10.
        /// </summary>
        /// <returns>Una lista de objetos Contribuyente.</returns>
        public async Task<IEnumerable<Contribuyente>> GetLast10AddedContribuyentesAsync(int idMunicipio)
        {
            return await _context.Contribuyentes
                .Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1) // Filtra por idMunicipio y por EstadoId igual a 1 (activos)
                .OrderByDescending(c => c.Id) // Ordena por Id de forma descendente (los más nuevos tienen Id más alto)
                .Take(20)                     // Toma solo los primeros 10
                .ToListAsync();
        }

        public async Task<IEnumerable<Contribuyente>> SearchContribuyentesByNameAsync(string nombreBusqueda, int idMunicipio)
        {
            // Usamos ToLower() para que la búsqueda sea insensible a mayúsculas/minúsculas
            // y .Contains() para buscar el fragmento en cualquier parte de Nombres o Apellidos.
            return await _context.Contribuyentes
                .Where(c => c.IdMunicipio == idMunicipio && c.Nombres.ToLower().Contains(nombreBusqueda.ToLower()) ||
                            c.Apellidos.ToLower().Contains(nombreBusqueda.ToLower()) ||
                            c.NumeroDocumento.ToLower().Contains(nombreBusqueda.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Contribuyente>> Contribuyentes3MesesAdeudados(int idMunicipio)
        {
            // 1. Obtener todos los pares (IdInmueble, Periodo, IdContribuyente) de los tributos que se consideran PAGADOS.
            // Se asume que un pago indica que el IdInmueble y Periodo correspondiente ha sido cubierto.
            // Esto es crucial para la lógica de "si Y SOLO SI no existe".
            var paidPeriodsByInmueble = await _context.Pagos
                .Where(p => p.IdMunicipio == idMunicipio && p.MontoPagado > 0 && p.FechaPago.HasValue && p.EstadoId == 1) // Asumo EstadoId=1 para pago exitoso
                .Select(p => new { p.Idinmueble, p.Periodo, IdContribuyente = p.IdContribuyente ?? 0 }) // Asegurarse de manejar IdContribuyente nulo si aplica
                .Distinct()
                .ToListAsync();

            // 2. Encontrar todos los tributos que están vencidos y NO tienen un pago asociado
            // para su IdInmueble y Periodo.
            // Se considera "más de 3 meses" si hay al menos 4 periodos distintos adeudados.
            var unpaidOverdueTributos = await _context.Tributos
                .Where(t => t.IdMunicipio == idMunicipio && t.FechaVencimiento < DateTime.Today)
                .ToListAsync(); // Trae todos los tributos vencidos y no pagados (según su estado)

            // Ahora, filtramos estos tributos para excluir aquellos que tienen un registro de pago
            // en 'paidPeriodsByInmueble' para el mismo Inmueble, Contribuyente y Periodo.
            var trulyOverdueTributos = unpaidOverdueTributos
                .Where(t => !paidPeriodsByInmueble.Any(pp =>
                    pp.Idinmueble == t.IdInmueble &&
                    pp.Periodo == t.Periodo &&
                    pp.IdContribuyente == t.IdContribuyente
                ))
                .ToList();

            // 3. Agrupar por contribuyente y contar el número de periodos distintos adeudados
            var contribuyentesDeudoresIds = trulyOverdueTributos
                .GroupBy(t => t.IdContribuyente)
                .Where(g => g.Select(t => t.Periodo).Distinct().Count() >= 3) // Contar 3 o más periodos distintos adeudados
                .Select(g => g.Key)
                .ToList();

            // 4. Obtener los objetos Contribuyente completos de los IDs deudores
            return await _context.Contribuyentes
                .Where(c => c.IdMunicipio == idMunicipio && contribuyentesDeudoresIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(int idContribuyente, ContribuyenteDTO? dto, int idUsuario)
        {
            try
            {
                var contrib = await _context.Contribuyentes.FindAsync(idContribuyente);
                if (contrib == null || dto == null)
                {
                    return false; // No se encontró el contribuyente o el DTO es nulo
                }
                // Actualizar solo las propiedades que no son nulas en el DTO
                if (dto.IdLocalidad != null) contrib.IdLocalidad = dto.IdLocalidad;
                if (dto.IdTipoDocumento != null) contrib.IdTipoDocumento = dto.IdTipoDocumento;
                if (dto.NumeroDocumento != null) contrib.NumeroDocumento = dto.NumeroDocumento;
                if (dto.CUIL != null) contrib.CUIL = dto.CUIL;
                if (dto.Nombres != null) contrib.Nombres = dto.Nombres;
                if (dto.Apellidos != null) contrib.Apellidos = dto.Apellidos;
                if (dto.Calle != null) contrib.Calle = dto.Calle;
                if (dto.Numero != null) contrib.Numero = dto.Numero;
                if (dto.Orientacion != null) contrib.Orientacion = dto.Orientacion;
                if (dto.Referencias != null) contrib.Referencias = dto.Referencias;
                if (dto.Telefono != null) contrib.Telefono = dto.Telefono;
                if (dto.Celular != null) contrib.Celular = dto.Celular;
                if (dto.Email != null) contrib.Email = dto.Email;
                contrib.FecModifica = DateTime.Now;
                contrib.OpModifica = idUsuario;

                _context.Contribuyentes.Update(contrib);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar el contribuyente: {ex.Message}", ex);
            }
        }
    }
}
