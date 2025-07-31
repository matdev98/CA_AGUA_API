using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<UsuarioRol> _baseRepository;

        public UserService(IUserRepository userRepository, IBaseRepository<UsuarioRol> baseRepository, IRolRepository rolRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _rolRepository = rolRepository;
            _baseRepository = baseRepository;
        }

        public async Task<ResultadoDTO<UserDTO>> ObtenerUserPorIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ResultadoDTO<UserDTO>.Fallido($"User con ID {id} no encontrado.");

            var userDto = _mapper.Map<UserDTO>(user);
            return ResultadoDTO<UserDTO>.Exitoso(userDto);
        }

        public async Task<ResultadoDTO<IEnumerable<UserDTO>>> ObtenerTodosLosUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return ResultadoDTO<IEnumerable<UserDTO>>.Exitoso(usersDto);
        }

        public async Task<Rol> GetRolByIdUsuario(int id)
        {
            var rol = await _userRepository.GetRolByIdUsuario(id);
            return rol;
        }

        public async Task<string> GetNombreRol(int idUsuario)
        {
            var nombreRol = await _userRepository.GetNombreRol(idUsuario);
            return nombreRol;
        }

        public async Task<List<UserDTO>> GetUsersMunicipio(int idMunicipio)
        {
            var users = await _userRepository.GetUsersByMunicipioAsync(idMunicipio);
            return users;
        }

        public async Task<bool> CheckUsername(string username, string email)
        {
            var exists = await _userRepository.CheckUsername(username, email);
            return exists;
        }

        public async Task<bool> CheckUsername(string username, string email, int id)
        {
            var exists = await _userRepository.CheckUsername(username, email, id);
            return exists;
        }

        public async Task<Usuarios> CreateUser(Usuarios entity)
        {
            await _userRepository.AddAsync(entity);
            return entity;
        }

        public async Task<bool> CambiarRol(int id, int rol, int idUser)
        {
            if (rol == 0)
            {
                return false;
            }
            else
            {
                var usuarioRol = await _userRepository.GetUsuarioRol(id);
                if (usuarioRol.IdRol == rol)
                {
                    return false;
                }
                else
                {
                    if (usuarioRol.IdRol == 0)
                    {
                        var entity = _mapper.Map<UsuarioRol>(new UsuarioRol { IdUsuario = id, IdRol = rol, OpCrea = idUser });
                        var createdEntity = await _baseRepository.AddAsync(entity);
                    }
                    else
                    {
                        var deleted = await _rolRepository.DeleteUserRolAsync(id, usuarioRol.IdRol);
                        if (!deleted)
                        {
                            return false;
                        }
                        else
                        {
                            var entity = _mapper.Map<UsuarioRol>(new UsuarioRol { IdUsuario = id, IdRol = rol, OpCrea = idUser });
                            var createdEntity = await _baseRepository.AddAsync(entity);
                        }
                    }
                    return true;
                }
            }
        }
    }
}
