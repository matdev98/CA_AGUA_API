using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Noticias
    {
        public int Id { get; set; }
        public int IdMunicipio { get; set; } = 0;
        public string Descripcion { get; set; } = string.Empty;
        public string FechaDesde { get; set; } = "190001";
        public string FechaHasta { get; set; } = "190001";
        public bool Anulado { get; set; } = false;
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpModi { get; set; } = 0;
        public DateTime FecModi { get; set; } = new DateTime(1900,1,1);
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900,1,1);
    }
}
