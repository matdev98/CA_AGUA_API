using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class UsuarioConTokenDTO
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public bool Activo { get; set; }
        public string Token { get; set; } = null!;
    }
}
