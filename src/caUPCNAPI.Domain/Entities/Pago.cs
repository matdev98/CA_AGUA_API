using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int? IdContribuyente { get; set; }
        public int? IdTributo { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? MontoPagado { get; set; }
        public int? IdMedioPago { get; set; }
        public int Idinmueble {  get; set; }
        public string Periodo { get; set; }
        public int IdCierre { get; set; }
        public int EstadoId { get; set; }

    }
}
