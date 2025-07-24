using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ResultadoDTO<UserDTO>> ObtenerUserPorIdAsync(int id);
        Task<ResultadoDTO<IEnumerable<UserDTO>>> ObtenerTodosLosUsersAsync();
        Task<Rol> GetRolByIdUsuario(int id);
    }
}
