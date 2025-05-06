using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class PlantillaNotificacionDTO
    {
        public string Titulo { get; set; } = null!;
        public string Cuerpo { get; set; } = null!;
        public string CanalDestino { get; set; } = null!;
        public bool Activo { get; set; }
    }

}
