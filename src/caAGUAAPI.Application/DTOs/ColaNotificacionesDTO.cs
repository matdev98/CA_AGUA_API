using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
{
    public class ColaNotificacionesDTO
    {
        public int IdPlantilla { get; set; }
        public int IdContribuyente { get; set; }
        public string CelularDestino { get; set; } = null!;
        public string Canal { get; set; } = null!;
        public DateTime FechaProgramada { get; set; }
        public string EstadoEnvio { get; set; } = null!;
        public int Intento { get; set; }
    }


}
