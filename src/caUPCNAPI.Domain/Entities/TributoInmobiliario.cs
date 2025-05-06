using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class TributoInmobiliario
    {
        public int IdTributo { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdInmueble { get; set; }
        public int? IdTipoImpuesto { get; set; }
        public string? Descripcion { get; set; }
        public string? Periodo { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public bool? Pagado { get; set; }
        public int? IdEstado { get; set; }
    }
}
