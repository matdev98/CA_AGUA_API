using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IInicioCajaService : IBaseService<InicioCaja>
    {
        Task<InicioCaja> ProcesarInicioDeCajaAsync(int idUsuario, int idMunicipio, decimal monto);
        Task<IEnumerable<InicioCaja>> ObtenerInicioCajaPeriodoAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<bool> UpdateInicioCajaEstadoIdAsync(int id);
    }

}
