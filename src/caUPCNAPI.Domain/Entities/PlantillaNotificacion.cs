using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class PlantillaNotificacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Cuerpo { get; set; } = null!;
        public string CanalDestino { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
