using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Services
{
    public interface ICierreCajaService : IBaseService<CierreCaja>
    {
        Task<CierreCaja> ProcesarCierreDeCajaAsync(int idUsuario, int idMunicipio);
        Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosCerradosAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosSinCerrarAsync(int idMunicipio);
        Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosDeUnCierreAsync(int idMunicipio, int idCierre);
        Task<IEnumerable<CierreCaja>> ObtenerCierreCajaPeriodoAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta);
        Task<bool> UpdateCierreCajaEstadoIdAsync(int id);
        Task<byte[]> GenerarCierreCajaPdf(int idMunicipio, int idCierre, IEnumerable<PagoCerradoDetalleDTO> resultadoDTO);
    }

}
