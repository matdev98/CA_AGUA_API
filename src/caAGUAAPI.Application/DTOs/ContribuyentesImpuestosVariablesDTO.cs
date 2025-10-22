using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class ContribuyentesImpuestosVariablesDTO
    {
        public int IdContribuyente { get; set; }
        public int IdTipoImpuesto { get; set; }
        public int IdInmueble { get; set; }
        public DateTime PeriodoDesde { get; set; }
        public DateTime PeriodoHasta { get; set; }
    }


}
