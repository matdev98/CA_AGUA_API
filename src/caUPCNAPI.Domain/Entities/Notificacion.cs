using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int IdContribuyente { get; set; }
        public string CelularDestino { get; set; } = null!;
        public string Canal { get; set; } = null!;
        public DateTime FechaEmision { get; set; }
        public string Mensaje { get; set; } = null!;
        public bool Enviada { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leida { get; set; }
    }
}
