using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string NombrePermiso { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
