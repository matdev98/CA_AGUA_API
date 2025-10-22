using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Domain.Entities
{
    public class InmuebleObra
    {
        public int Id { get; set; }
        public int ObraId { get; set; }
        public decimal PorcentajeParticipacion { get; set; }
    }
}
