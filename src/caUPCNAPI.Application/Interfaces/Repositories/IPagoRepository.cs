using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IPagoRepository : IBaseRepository<Pago>
    {
        Task<bool> UpdateEstadoIdAsync(int id);
        Task<List<Pago>> PagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);
        Task<Pago> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio);
        Task<CierreCaja> RealizarCierreDeCajaAsync(int idUsuario, int idMunicipio);
    }
}
