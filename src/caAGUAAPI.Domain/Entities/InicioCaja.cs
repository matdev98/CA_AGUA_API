using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class InicioCaja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdMunicipio { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreCompleto { get; set; } = null!;

        [Required]
        public DateTime FechaInicioCaja { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Monto { get; set; }

        public int EstadoId { get; set; }
    }
}
