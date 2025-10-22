using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class CantidadDTO
    {
        public int Cantidad { get; set; }
    }

    public class InmueblesPorTipoDTO
    {
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
    }

    public class TopDeudoresDTO
    {
        public int IdContribuyente { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Documento { get; set; } = null!;
        public decimal DeudaTotal { get; set; }
    }

}
