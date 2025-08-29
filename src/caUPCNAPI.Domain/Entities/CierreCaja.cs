using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class CierreCaja
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdMunicipio { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public int EstadoId { get; set; }
        public bool? Anulado { get; set; } = false;
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
    }
}
