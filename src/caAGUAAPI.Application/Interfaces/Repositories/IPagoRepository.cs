using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IPagoRepository : IBaseRepository<Pago>
    {
        Task<bool> UpdateEstadoIdAsync(int id, int idUsuario);
        Task<List<Pago>> PagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);
        Task<Pago> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio);
        Task<CierreCaja> RealizarCierreDeCajaAsync(int idUsuario, int idMunicipio);
        Task<bool> AnularCierreCajaAsync(int idCierrre, int idUsuario);
        Task<bool> Update(int id, Pago entidad);
        Task<Pago> GetById(int id);
    }
}
