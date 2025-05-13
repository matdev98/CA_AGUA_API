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
        public int? EstadoId { get; set; }
        public int NumeroContribuyente { get; set; }
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
