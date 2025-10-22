using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class ObraPublica
    {
        public int IdObra { get; set; }
        public int? IdMunicipio { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? CostoTotal { get; set; }
        public int? IdEstado { get; set; }
    }
}
