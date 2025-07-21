using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Inmueble
    {
        public int Id { get; set; }
        public int IdContribuyente { get; set; }
        public int IdTipoInmueble { get; set; }
        public int IdLocalidad { get; set; }
        public string Calle { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Orientacion { get; set; } = null!;
        public string Referencias { get; set; } = null!;
        public decimal EvaluoFiscal { get; set; }
        public int EstadoId { get; set; }
        public decimal AreaTotal { get; set; }
        public int IdMunicipio { get; set; }
    }
}
