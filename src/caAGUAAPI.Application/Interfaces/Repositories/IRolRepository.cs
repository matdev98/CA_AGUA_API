using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IRolRepository
    {
        Task<Rol?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Rol dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteUserRolAsync(int idUsuario, int idRol);
        Task<bool> DeleteRolPermisoAsync(int idRol, int idUsuario);
        Task<List<string>> GetPermisosRol(int idRol);
        Task<List<UserDTO>> GetUsersByRol(int idRol);
        Task<int> GetIdRol(string nombreRol);
    }
}
