using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IValorTipoImpuestoRepository : IBaseRepository<ValorTipoImpuesto>
    {
        Task<IEnumerable<NombreTipoImpuestoDTO>> GetNombreTipoImpuesto();
    }
}
