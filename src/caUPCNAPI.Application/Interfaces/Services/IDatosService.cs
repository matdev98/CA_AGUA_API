using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IDatosService
    {
        Task<int> CantidadContribuyentesActivosAsync(int idMunicipio);
        Task<int> TotalInmueblesRegistradosAsync(int idMunicipio);
        Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync(int idMunicipio);
        Task<decimal> DeudaTotalAcumuladaAsync(int idMunicipio);
        Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync(int idMunicipio);
    }

}
