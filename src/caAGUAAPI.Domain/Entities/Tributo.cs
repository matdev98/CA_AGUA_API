using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class Tributo
    {
        public int Id { get; set; }
        public int IdMunicipio { get; set; }
        public int IdContribuyente { get; set; }
        public int IdInmueble { get; set; }
        public int IdTipoImpuesto { get; set; }
        public string Descripcion { get; set; } = null!;
        public string Periodo { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int IdEstado { get; set; }
        public int IdEstadoTributo { get; set; }
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpMod { get; set; } = 0;
        public DateTime FecMod { get; set; } = new DateTime(1900, 1, 1);
        public bool Anulado { get; set; } = false; 
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
    }
}
