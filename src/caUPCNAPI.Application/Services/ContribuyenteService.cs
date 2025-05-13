using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class ContribuyenteService : BaseService<Contribuyente>, IContribuyenteService
    {
        private readonly IContribuyenteRepository _repository;

        public ContribuyenteService(IContribuyenteRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _repository.GetByMunicipioIdAsync(idMunicipio);
        }

    }

}
