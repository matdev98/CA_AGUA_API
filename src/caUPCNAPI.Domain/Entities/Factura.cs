using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Factura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdContribuyente { get; set; }

        public string Periodo { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal MontoTotal { get; set; }

        public string Estado { get; set; }

        public DateTime? FechaPago { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int IdMunicipio { get; set; }
        public string codigobarra { get; set; }

    }
}
