using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class ContribuyenteDTO
    {
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
        public int? EstadoId { get; set; }
    }

    //public class ContribuyenteDTO
    //{
    //    public int IdMunicipio { get; set; }
    //    public int IdLocalidad { get; set; }
    //    public int IdTipoDocumento { get; set; }

    //    public string NumeroDocumento { get; set; } = "";
    //    public string CUIL { get; set; } = "";

    //    public string Nombres { get; set; } = null!; // Obligatorio
    //    public string Apellidos { get; set; } = null!; // Obligatorio

    //    public string Calle { get; set; } = "";
    //    public string Numero { get; set; } = "";
    //    public string Orientacion { get; set; } = "";
    //    public string Referencias { get; set; } = "";
    //    public string Telefono { get; set; } = "";
    //    public string Celular { get; set; } = "";
    //    public string Email { get; set; } = "";

    //    public int? EstadoId { get; set; }
    //}


}
