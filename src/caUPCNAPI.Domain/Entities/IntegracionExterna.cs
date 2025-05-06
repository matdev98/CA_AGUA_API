using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class IntegracionExterna
    {
        public int IdIntegracion { get; set; }
        public string NombreSistema { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string UrlApi { get; set; } = null!;
        public string TokenAcceso { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
    }
}
