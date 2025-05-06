using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class ContribuyentesImpuestosVariables
    {
        public int Id { get; set; }
        public int IdContribuyente { get; set; }
        public int IdTipoImpuesto { get; set; }
        public int IdInmueble { get; set; }
        public DateTime PeriodoDesde { get; set; }
        public DateTime PeriodoHasta { get; set; }
    }
}
