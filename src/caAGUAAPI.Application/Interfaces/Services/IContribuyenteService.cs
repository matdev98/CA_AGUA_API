using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Services
{
    public interface IContribuyenteService : IBaseService<Contribuyente>
    {
        Task<Contribuyente?> GetContribuyenteByDNI(string documento);
        Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio);
        Task<bool> UpdateContribuyenteEstadoIdAsync(int id, int idUsuario);
        Task<IEnumerable<Contribuyente>> GetLast10ContribuyentesAsync(int idMunicipio);
        Task<IEnumerable<Contribuyente>> SearchContribuyentesAsync(string nombreBusqueda, int idMunicipio);
        Task<IEnumerable<Contribuyente>> Contribuyentes3MesesAdeudados(int idMunicipio);
        Task<bool> UpdateAsync(int idContribuyente, ContribuyenteDTO? contribuyente, int idUsuario);
    }

}
