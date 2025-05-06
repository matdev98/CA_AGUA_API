using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class LoginRequest
    {
        public string Usuario { get; set; } = null!;
        public string Clave { get; set; } = null!;
    }
}
