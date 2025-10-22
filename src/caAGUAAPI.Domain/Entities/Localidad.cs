using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class Localidad
    {
        public int Id { get; set; }
        public string Departamento { get; set; } = null!;
        public string Provincia { get; set; } = null!;
        public string CodigoPostal { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int IdMunicipio { get; set; }
    }
}
