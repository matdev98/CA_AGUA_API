using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;

namespace caAGUAAPI.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Usuarios> ValidarCredencialesAsync(string usuario, string clave);
        Task<bool> RegistrarUsuarioAsync(RegistroRequest registro);
        string GenerarToken(Usuarios usuario);
        string ObtenerRolesUsuario(int usuarioId);
        List<string> ObtenerPermisosRol(int usuarioId);
    }

}
