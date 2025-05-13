using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Enums
{
    public enum EstadoTributoEnum
    {
        Pagada = 1,
        Pendiente = 6,
        ParcialmentePagado = 4,
        ParcialmentePagadoVencido = 5,
        VencidoSinPago = 3
    }
}
