using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface ITributoRepository : IBaseRepository<Tributo>
    {
        Task<IEnumerable<TributoContribuyenteDTO>> GetByContribuyenteIdAsync(int contribuyenteId);
        Task GenerarTributosDelMesAsync(int IdMunicipio);
        Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupados(int idContribuyente, string periodo);
        Task<List<TributoContribuyenteDTO>> ObtenerDetalleTributoPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo);
        Task<List<TributoContribuyenteDTO>> ObtenerTributosPorPeriodoAsync(string periodo, int idMunicipio);
        Task<List<TributoContribuyenteDTO>> ObtenerTodosLosTributosDelContribuyentePorPeriodo(int idContribuyente, string periodo);
    }
}
