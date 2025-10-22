using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class UserConRolDTO
    {
        public int Id { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
        public int IdMunicipio { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}
