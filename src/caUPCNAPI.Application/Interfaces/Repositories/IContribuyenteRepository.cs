using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IContribuyenteRepository : IBaseRepository<Contribuyente>
    {
        Task<Contribuyente?> GetContribuyenteByDNI(string documento);
        Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio);
        Task<bool> UpdateEstadoIdAsync(int id, int idUsuario);
        Task<IEnumerable<Contribuyente>> GetLast10AddedContribuyentesAsync(int idMunicipio);
        Task<IEnumerable<Contribuyente>> SearchContribuyentesByNameAsync(string nombreBusqueda, int idMunicipio);
        Task<IEnumerable<Contribuyente>> Contribuyentes3MesesAdeudados(int idMunicipio);
        Task<bool> UpdateAsync(int idContribuyente, ContribuyenteDTO? contribuyente, int idUsuario);
    }
}
