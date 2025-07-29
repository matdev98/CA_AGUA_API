using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
        public int IdMunicipio { get; set; }
    }

    public class UserCreateDTO
    {
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? ClaveHash { get; set; }
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
        public int idMunicipio { get; set; }
        public int IdRol { get; set; }
    }

    public class UserUpdateDTO
    {
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? ClaveHash { get; set; }
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
    }


}
