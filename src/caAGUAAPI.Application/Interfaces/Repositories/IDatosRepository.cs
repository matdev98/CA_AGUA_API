using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IDatosRepository 
    {
        Task<int> GetCantidadContribuyentesActivosAsync(int idMunicipio);
        Task<int> TotalInmueblesRegistradosAsync(int idMunicipio);
        Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync(int idMunicipio);
        Task<decimal> DeudaTotalAcumuladaAsync(int idMunicipio);
        Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync(int idMunicipio);

    }
}
