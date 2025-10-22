using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class InteresMoraDTO
    {
        public int TributoId { get; set; }
        public int DiasMora { get; set; }
        public decimal MontoInteres { get; set; }
        public DateTime CalculadoEn { get; set; }
    }


}
