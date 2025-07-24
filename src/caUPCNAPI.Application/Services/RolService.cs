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
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public RolService(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<Rol> GetByIdAsync(int id)
        {
            var rol = await _rolRepository.GetByIdAsync(id);
            return rol;
        }

        public async Task<bool> UpdateAsync(int id, Rol dto)
        {
            var exito = await _rolRepository.UpdateAsync(id, dto);
            return exito;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exito = await _rolRepository.DeleteAsync(id);
            return exito;
        }

        public async Task<bool> DeleteUserRolAsync(int idUsuario, int idRol)
        {
            var exito = await _rolRepository.DeleteUserRolAsync(idUsuario, idRol);
            return exito;
        }

        public async Task<bool> DeleteRolPermisoAsync(int idRol, int idPermiso)
        {
            var exito = await _rolRepository.DeleteRolPermisoAsync(idRol, idPermiso);
            return exito;
        }
    }
}
