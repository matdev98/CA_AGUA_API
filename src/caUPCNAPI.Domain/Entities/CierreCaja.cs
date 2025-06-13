using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class CierreCaja
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdMunicipio { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
