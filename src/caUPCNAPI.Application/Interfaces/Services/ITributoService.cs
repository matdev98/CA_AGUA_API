using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface ITributoService : IBaseService<Tributo>
    {
        Task<ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>> GetByContribuyenteIdAsync(int contribuyenteId);

        Task GenerarTributosDelMesAsync(int IdMunicipio, int idUsuario);

        Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupadosAsync(int idContribuyente, string periodo);
        Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupadosSinPeriodoAsync(int idContribuyente);
        Task<List<TributoContribuyenteDTO>> ObtenerDetalleTributoPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);

        Task<List<TributoContribuyenteDTO>> ObtenerTributosPorPeriodoAsync(string periodo, int idMunicipio);
        Task<List<TipoImpuesto>> GetFijos(int idMunicipio);
        Task<bool> ApplyToAll(TipoImpuesto impuesto, int idMunicipio);
    }

}
