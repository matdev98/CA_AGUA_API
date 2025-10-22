using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class NombreTipoImpuestoDTO
    {
        public int TipoImpuestoId { get; set; }
        public string NombreTipoImpuesto { get; set; } = null!;
        public DateTime PeriodoDesde { get; set; }
        public DateTime PeriodoHasta { get; set; }
        public decimal Valor { get; set; }
        public string Resolucion { get; set; } = null!;
    }


}
