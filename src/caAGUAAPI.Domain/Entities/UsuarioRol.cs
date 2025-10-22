using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class UsuarioRol
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpCrea { get; set; } = 0;
        public bool Anulado { get; set; } = false;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
        public int OpAnula { get; set; } = 0;
    }
}
