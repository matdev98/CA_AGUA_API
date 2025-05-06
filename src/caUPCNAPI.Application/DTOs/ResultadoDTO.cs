using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class ResultadoDTO<T>
    {
        public bool EsExitoso { get; set; } = true;
        public T? Datos { get; set; } // El payload de datos reales
        public List<string>? Errores { get; set; } // Lista de errores si EsExitoso = false
        public string? Mensaje { get; set; } // Mensaje opcional

        // Métodos estáticos opcionales para facilitar la creación
        public static ResultadoDTO<T> Exitoso(T datos, string? mensaje = null) =>
            new ResultadoDTO<T> { EsExitoso = true, Datos = datos, Mensaje = mensaje };

        public static ResultadoDTO<T> Fallido(List<string> errores, string? mensaje = null) =>
            new ResultadoDTO<T> { EsExitoso = false, Errores = errores, Mensaje = mensaje };

        public static ResultadoDTO<T> Fallido(string error, string? mensaje = null) =>
            new ResultadoDTO<T> { EsExitoso = false, Errores = new List<string> { error }, Mensaje = mensaje };
    }
}
