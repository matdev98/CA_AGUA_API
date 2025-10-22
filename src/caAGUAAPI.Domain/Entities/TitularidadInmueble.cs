using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class TitularidadInmueble
    {
        public int IdTitularidad { get; set; }
        public int? IdInmueble { get; set; }
        public int? IdContribuyente { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
