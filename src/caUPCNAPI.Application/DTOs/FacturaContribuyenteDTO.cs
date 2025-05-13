using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class FacturaContribuyenteDTO
    {
        public int IdContribuyente { get; set; }
        public string NombreCompleto { get; set; } 
        public string Documento { get; set; }
        public string Periodo { get; set; }
        public List<InmuebleConTributosDTO> InmueblesConTributos { get; set; }
    }

    public class InmuebleConTributosDTO
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; }
        public List<TributoContribuyenteDTO> Tributos { get; set; }
        public decimal MontoTotalInmueble { get; set; }
    }
}
