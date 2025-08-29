using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Domain.Entities
{
    public class Recibo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdPago { get; set; }

        [Required]
        public DateTime FechaEmision { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MontoTotal { get; set; }

        [MaxLength(6)]
        public string? Periodo { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreContribuyente { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string DocumentoContribuyente { get; set; } = null!;

        [MaxLength(255)]
        public string? DireccionInmueble { get; set; }

        [Required]
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
