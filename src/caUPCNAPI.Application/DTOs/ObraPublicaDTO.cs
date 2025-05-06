using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class ObraPublicaDTO
    {
        public int? IdMunicipio { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? CostoTotal { get; set; }
        public int? IdEstado { get; set; }
    }


}
