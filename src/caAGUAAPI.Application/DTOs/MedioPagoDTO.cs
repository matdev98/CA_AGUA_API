using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class MedioPagoDTO
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Condiciones { get; set; } = null!;
        public bool TieneRecargo { get; set; }
        public decimal PorcentajeRecargo { get; set; }
        public bool TieneComision { get; set; }
        public decimal PorcentajeComision { get; set; }
        public string TipoIntegracion { get; set; } = null!;
        public string DetallesIntegracion { get; set; } = null!;
    }


}
