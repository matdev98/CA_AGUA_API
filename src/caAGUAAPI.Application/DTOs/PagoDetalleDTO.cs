using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class PagoDetalleDTO
    {
        public DateTime? FechaPago { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int IdInmueble { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Orientacion { get; set; }
        public string Departamento { get; set; }
        public string MedioPago { get; set; }
        public decimal? MontoPagado { get; set; }
    }

}
