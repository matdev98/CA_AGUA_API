using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface ICierreCajaRepository : IBaseRepository<CierreCaja>
    {
        Task<CierreCaja> RealizarCierreDeCajaAsync(int idUsuario, int idMunicipio);
        Task<IEnumerable<PagoCerradoDetalleDTO>> PagosCerrados(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<IEnumerable<PagoCerradoDetalleDTO>> PagosSinCerrar(int idMunicipio);
        Task<IEnumerable<PagoCerradoDetalleDTO>> PagosdeunCierre(int idMunicipio, int idCierre);
        Task<IEnumerable<CierreCaja>> GetCierreCajaPeriodo(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<bool> UpdateEstadoIdAsync(int id);
        Task<string> GetMunicipio(int idMunicipio);
        Task<string> GetFechaCierre(int idCierre);
        Task<string> GetLogoMunicipio(int idMunicipio);
    }
}
