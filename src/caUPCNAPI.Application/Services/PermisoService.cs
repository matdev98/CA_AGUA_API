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
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _permisoRepository;
        private readonly IMapper _mapper;

        public PermisoService(IPermisoRepository permisoRepository, IMapper mapper)
        {
            _permisoRepository = permisoRepository;
            _mapper = mapper;
        }

        public async Task<Permiso> GetByIdAsync(int id)
        {
            var rol = await _permisoRepository.GetByIdAsync(id);
            return rol;
        }

        public async Task<bool> UpdateAsync(int id, Permiso dto)
        {
            var exito = await _permisoRepository.UpdateAsync(id, dto);
            return exito;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exito = await _permisoRepository.DeleteAsync(id);
            return exito;
        }
    }
}
