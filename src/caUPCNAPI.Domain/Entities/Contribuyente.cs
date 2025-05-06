using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Contribuyente
    {
        public int Id { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdLocalidad { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = null!;
        public string CUIL { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Calle { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Orientacion { get; set; } = null!;
        public string Referencias { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? FechaAlta { get; set; }
        public int? EstadoId { get; set; }

    }
}