using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using MercadoPago.Resource.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IPagoService : IBaseService<Pago>
    {
        Task<bool> UpdateInmuebleEstadoIdAsync(int id, int idUsuario);
        Task<List<Pago>> ObtenerPagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);
        Task<bool> DeleteAsync(int id);
        Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio);
        Task<CierreCaja> ProcesarCierreDeCajaAsync(int idUsuario, int idMunicipio);
        Task<Preference> CrearPreferenciaAsync(string documento, string anioMes, string nombreProducto);
        Task<string> ObtenerTokenChattigoAsync();
        Task EnviarMensajeChattigoAsync(string token, string telefono, string nombre, string mensaje);
        Task<bool> AnularCierreCajaAsync(int idCierrre, int idUsuario);
        Task<bool> Update(int id, Pago entidad);
        Task<Pago> GetById(int id);
    }

}
