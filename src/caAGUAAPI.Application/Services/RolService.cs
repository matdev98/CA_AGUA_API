using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Services
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

        public async Task<List<string>> GetPermisosRol(int idRol)
        {
            var permisos = await _rolRepository.GetPermisosRol(idRol);
            return permisos;
        }

        public async Task<List<UserDTO>> GetUsersByRol(int idRol)
        {
            var users = await _rolRepository.GetUsersByRol(idRol);
            return users;
        }
    }
}
