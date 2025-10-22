using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IInicioCajaRepository : IBaseRepository<InicioCaja>
    {
        Task<InicioCaja> RealizarInicioDeCajaAsync(int idUsuario, int idMunicipio, decimal monto);
        Task<IEnumerable<InicioCaja>> GetInicioCajaPeriodo(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<bool> UpdateEstadoIdAsync(int id);

    }
}
