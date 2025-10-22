using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class GenerarReciboRequest
    {
        [Required(ErrorMessage = "El ID de Pago es obligatorio.")]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "El ID de Contribuyente es obligatorio.")]
        public int IdContribuyente { get; set; }
    }

}
