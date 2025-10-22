using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caAGUAAPI.Domain.Entities;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<Usuarios> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task<Usuarios> ObtenerPorEmailAsync(string email);
        Task CrearUsuarioAsync(Usuarios usuario);
        string ObtenerRolesUsuario(int usuarioId);
        List<string> ObtenerPermisosRol(int usuarioId);
    }

}
