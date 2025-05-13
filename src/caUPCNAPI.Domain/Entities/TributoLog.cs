using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class TributoLog
    {
        public int Id { get; set; }

        public int IdMunicipio { get; set; }

        public string Periodo { get; set; } = null!; // formato: "yyyyMM"

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        public int TotalContribuyentes { get; set; }

        public int TotalInmuebles { get; set; }

        public int TotalTributosGenerados { get; set; }
    }

}
