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

        public async Task<bool> UpdateEstadoIdAsync(int id)
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
    }
}
