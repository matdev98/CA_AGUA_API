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
    public class InmuebleRepository : BaseRepository<Inmueble>, IInmuebleRepository
    {
        private readonly AppDbContext _context;

        public InmuebleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inmueble>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _context.Inmuebles
                .Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inmueble>> SearchInmueblesByNameAsync(string nombre)
        {
            // Usamos ToLower() para que la búsqueda sea insensible a mayúsculas/minúsculas
            // y .Contains() para buscar el fragmento en cualquier parte de la calle.
            return await _context.Inmuebles
                .Where(i => i.Calle.ToLower().Contains(nombre.ToLower())) // Asumo que buscas por nombre de Calle
                .ToListAsync();
        }

        public async Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            var inmuebles = await _context.Inmuebles
                .Where(i => i.IdContribuyente == contribuyenteId && i.EstadoId == 1)
                .ToListAsync();

            var resultado = inmuebles.Select(i => new Inmueble
            {
                Id = i.Id,
                IdContribuyente = i.IdContribuyente,
                IdTipoInmueble = i.IdTipoInmueble,
                IdLocalidad = i.IdLocalidad,
                EvaluoFiscal = i.EvaluoFiscal,
                EstadoId = i.EstadoId,
                Referencias = i.Referencias,
                Calle = i.Calle,
                Numero = i.Numero,
                Orientacion = i.Orientacion,
                AreaTotal = i.AreaTotal
            });

            return resultado;
        }

        public async Task<bool> UpdateEstadoIdAsync(int id)
        {
            try
            {
                var inmueble = await _context.Inmuebles.FindAsync(id);

                if (inmueble == null)
                {
                    return false;
                }

                inmueble.EstadoId = 2;

                _context.Inmuebles.Update(inmueble);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Inmueble>> GetLastAddedInmueblesAsync(int idMunicipio)
        {
            return await _context.Inmuebles
                .Where(i => i.IdMunicipio == idMunicipio && i.EstadoId == 1)
                .OrderByDescending(i => i.Id) // Ordena por Id de forma descendente (los más nuevos tienen Id más alto)
                .Take(20)                     // Toma solo los primeros 10
                .ToListAsync();
        }
    }

}
