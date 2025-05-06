using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IDatosRepository 
    {
        Task<int> GetCantidadContribuyentesActivosAsync();
        Task<int> TotalInmueblesRegistradosAsync();
        Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync();
        Task<decimal> DeudaTotalAcumuladaAsync();
        Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync();

    }
}
