using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class AuditoriaDTO
    {
        public int UsuarioId { get; set; }
        public string TablaAfectada { get; set; } = null!;
        public int IdRegistroAfectado { get; set; }
        public string Accion { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string IpUsuario { get; set; } = null!;
        public string Modulo { get; set; } = null!;
        public DateTime FechaHora { get; set; }
    }


}
