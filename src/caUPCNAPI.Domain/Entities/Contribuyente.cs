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
        public string NumeroDocumento { get; set; } = string.Empty;
        public string CUIL { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Orientacion { get; set; } = string.Empty;
        public string Referencias { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? FechaAlta { get; set; }
        public int? EstadoId { get; set; }
        public int NumeroContribuyente { get; set; }
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public bool Anulado { get; set; } = false;
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
        public int OpModifica { get; set; } = 0;
        public DateTime FecModifica { get; set; } = new DateTime(1900, 1, 1);

    }
}