using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class TributoAgrupadoDTO
    {
        public int IdMunicipio { get; set; }
        public int IdContribuyente { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Documento { get; set; } = null!;
        public int IdInmueble { get; set; }
        public string Periodo { get; set; } = null!;
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public int IdEstado { get; set; }
        public string EstadoTributoDescripcion { get; set; } = null!;

    }

}
