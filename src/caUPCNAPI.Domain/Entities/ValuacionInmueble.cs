using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class ValuacionInmueble
    {
        public int IdValuacion { get; set; }
        public int? IdInmueble { get; set; }
        public DateTime? FechaValuacion { get; set; }
        public decimal? Evaluo { get; set; }
        public string? Fuente { get; set; }
        public string? Resolucion { get; set; }
    }
}
