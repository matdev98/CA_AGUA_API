using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class ValorTipoImpuesto
    {
        public int Id { get; set; }
        public int TipoImpuestoId { get; set; }
        public DateTime PeriodoDesde { get; set; }
        public DateTime PeriodoHasta { get; set; }
        public decimal Valor { get; set; }
        public string Resolucion { get; set; } = null!;
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpMod { get; set; } = 0;
        public DateTime FecMod { get; set; } = new DateTime(1900, 1, 1);
        public bool Anulado { get; set; } = false;
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
    }
}
