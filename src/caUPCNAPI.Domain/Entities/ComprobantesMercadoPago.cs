using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class ComprobantesMercadoPago
    {
        public int Id { get; set; }
        public int IdPago { get; set; }
        public string ComprobanteMp { get; set; } = null!;
        public decimal ImportePago { get; set; }
        public DateTime FechaCrea { get; set; }
        public DateTime? FechaAnula { get; set; }
    }
}
