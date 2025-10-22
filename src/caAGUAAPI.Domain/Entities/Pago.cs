using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
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
        public int IdMunicipio { get; set; }
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpMod { get; set; } = 0;
        public DateTime FecMod { get; set; } = new DateTime(1900, 1, 1);
        public bool Anulado { get; set; } = false;
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
    }
}
