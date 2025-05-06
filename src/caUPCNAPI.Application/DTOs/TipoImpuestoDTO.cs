using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class TipoImpuestoDTO
    {
        public int MunicipioId { get; set; }
        public string Descripcion { get; set; } = null!;
        public int PeriodicidadId { get; set; }
        public bool TieneTablaValores { get; set; }
        public string ResolucionAsociada { get; set; } = null!;
        public decimal ValorFijo { get; set; }
    }


}
