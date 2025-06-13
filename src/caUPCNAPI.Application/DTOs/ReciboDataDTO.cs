using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class ReciboDataDTO
    {
        public Pago Pago { get; set; } = null!; 
        public Tributo Tributo { get; set; } = null!; 
        public Contribuyente Contribuyente { get; set; } = null!; 
        public Inmueble Inmueble { get; set; } = null!;
        public MedioPago MedioPago { get; set; } = null!;
    }

}
