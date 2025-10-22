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
    public class InicioCajaRepository : BaseRepository<InicioCaja>, IInicioCajaRepository
    {
        private readonly AppDbContext _context;

        public InicioCajaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<InicioCaja> RealizarInicioDeCajaAsync(int idUsuario, int idMunicipio, decimal monto)
        {
            try
            {

                var usuario = await _context.Usuarios
                    .Where(p => p.Id == idUsuario) 
                    .FirstOrDefaultAsync();

                var nuevoInicio = new InicioCaja
                {
                   IdMunicipio = idMunicipio,
                   IdUsuario = idUsuario,
                   Monto = monto,
                   FechaInicioCaja = DateTime.Now,
                   NombreCompleto = usuario.NombreCompleto,
                   EstadoId = 1
                };

                _context.InicioCaja.Add(nuevoInicio);
                await _context.SaveChangesAsync();

                return nuevoInicio; 
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al realizar el cierre de caja.", ex); 
            }
        }

        public async Task<IEnumerable<InicioCaja>> GetInicioCajaPeriodo(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _context.InicioCaja
                .Where(cc => cc.IdMunicipio == idMunicipio && cc.EstadoId == 1 && cc.FechaInicioCaja.Date >= fechaDesde.Date && cc.FechaInicioCaja.Date <= fechaHasta.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateEstadoIdAsync(int id)
        {
            try
            {
                // Obtener el contribuyente por su Id
                var inicioCaja = await _context.InicioCaja.FindAsync(id);

                if (inicioCaja == null)
                {
                    // El contribuyente no fue encontrado
                    return false;
                }

                // Actualizar solo la propiedad EstadoId
                inicioCaja.EstadoId = 2;

                // Marcar la entidad como modificada y guardar los cambios
                _context.InicioCaja.Update(inicioCaja); // O _context.Entry(contribuyente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw; // Relanza la excepción para que el servicio la capture
            }
        }
    }
}
