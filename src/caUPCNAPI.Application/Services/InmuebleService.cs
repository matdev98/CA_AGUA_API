using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class InmuebleService : BaseService<Inmueble>, IInmuebleService
    {
        private readonly IInmuebleRepository _repository;

        public InmuebleService(IInmuebleRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Inmueble>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _repository.GetByMunicipioIdAsync(idMunicipio);
        }

        public async Task<IEnumerable<Inmueble>> SearchInmueblesAsync(string nombre)
        {

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
            {
                
                throw new ApplicationException("El término de búsqueda debe tener al menos 3 caracteres.");
            }

            try
            {
                var inmuebles = await _repository.SearchInmueblesByNameAsync(nombre);

                return inmuebles;
            }

            catch (Exception ex)
            {
               
                throw new ApplicationException($"Error al buscar inmuebles por nombre: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            return await _repository.GetByContribuyenteIdAsync(contribuyenteId);
        }

        public async Task<bool> UpdateInmuebleEstadoIdAsync(int id)
        {
            try
            {
                // Llama directamente al método del repositorio específico
                bool success = await _repository.UpdateEstadoIdAsync(id);
                return success;
            }
            catch (Exception ex)
            {
                throw; 
            }
        }

        public async Task<IEnumerable<Inmueble>> GetLastInmueblesAsync(int idMunicipio)
        {
            try
            {

                var inmuebles = await _repository.GetLastAddedInmueblesAsync(idMunicipio);
                return inmuebles;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener los inmuebles recientes.", ex);
            }
        }
    }

}


