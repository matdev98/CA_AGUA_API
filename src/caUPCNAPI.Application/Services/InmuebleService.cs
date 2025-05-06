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
    }

}


