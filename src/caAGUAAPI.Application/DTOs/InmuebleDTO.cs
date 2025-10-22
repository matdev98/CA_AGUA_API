using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class InmuebleDTO
    {
        public int? IdContribuyente { get; set; }
        public int? IdTipoInmueble { get; set; }
        public int? IdLocalidad { get; set; }
        public string? Calle { get; set; }
        public string? Numero { get; set; }
        public string? Orientacion { get; set; }
        public string? Referencias { get; set; }
        public decimal? EvaluoFiscal { get; set; }
        public int? EstadoId { get; set; }
        public decimal? AreaTotal { get; set; }
    }


}
