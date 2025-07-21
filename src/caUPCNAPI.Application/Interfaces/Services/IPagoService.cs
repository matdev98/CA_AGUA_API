using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IPagoService : IBaseService<Pago>
    {
        Task<bool> UpdateInmuebleEstadoIdAsync(int id);
        Task<List<Pago>> ObtenerPagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);
        Task<bool> DeleteAsync(int id);
        Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio);
        Task<CierreCaja> ProcesarCierreDeCajaAsync(int idUsuario, int idMunicipio);
    }

}
