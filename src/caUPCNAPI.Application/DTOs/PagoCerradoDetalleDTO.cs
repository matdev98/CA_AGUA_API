using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class PagoCerradoDetalleDTO
    {
        public int IdPago { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? MontoPagado { get; set; }
        public string Periodo { get; set; } = null!;
        public int IdCierre { get; set; }
        public int IdMunicipio { get; set; }

        // Datos del Contribuyente
        public int IdContribuyente { get; set; }
        public string NombresContribuyente { get; set; } = null!;
        public string ApellidosContribuyente { get; set; } = null!;
        public string NumeroDocumentoContribuyente { get; set; } = null!;

        // Datos del Medio de Pago
        public int IdMedioPago { get; set; }
        public string NombreMedioPago { get; set; } = null!;

        // Datos del Inmueble
        public int IdInmueble { get; set; }
        public string DireccionInmueble { get; set; } = null!;
    }

}
