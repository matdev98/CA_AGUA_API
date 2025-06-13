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
    }

}


