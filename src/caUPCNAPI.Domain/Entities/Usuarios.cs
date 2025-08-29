using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ClaveHash { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public bool Activo { get; set; }
        public int IdMunicipio { get; set; }
        public int OpCrea { get; set; } = 0;
        public DateTime FecCrea { get; set; } = DateTime.Now;
        public int OpMod { get; set; } = 0;
        public DateTime FecMod { get; set; } = new DateTime(1900, 1, 1);
        public bool Anulado { get; set; } = false;
        public int OpAnula { get; set; } = 0;
        public DateTime FecAnula { get; set; } = new DateTime(1900, 1, 1);
    }
}
