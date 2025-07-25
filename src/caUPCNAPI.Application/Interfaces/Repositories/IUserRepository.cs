using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<Usuarios?> GetByIdAsync(int id);
        Task<IEnumerable<Usuarios>> GetAllAsync();
        Task AddAsync(Usuarios user);
        Task UpdateAsync(Usuarios user);
        Task DeleteAsync(int id);
        Task<Rol> GetRolByIdUsuario(int id);
        Task<string> GetNombreRol(int idUsuario);
        Task<List<UserDTO>> GetUsersByMunicipioAsync(int idMunicipio);
    }
}
