using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class RegistroRequest
    {
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public int IdMunicipio { get; set; }
    }
}
