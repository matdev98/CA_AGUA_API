using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Municipio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Provincia { get; set; } = null!;
        public string Departamento { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Cuit { get; set; } = null!;
        public string Responsable { get; set; } = null!;
        public string PaginaWeb { get; set; } = null!;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoMunicipio { get; set; } = null!;
        public string Jurisdiccion { get; set; } = null!;
    }
}
