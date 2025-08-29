using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IInmuebleService : IBaseService<Inmueble>
    {
        Task<IEnumerable<Inmueble>> GetByMunicipioIdAsync(int idMunicipio);
        Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId);
        Task<bool> UpdateInmuebleEstadoIdAsync(int id, int idUsuario);
        Task<IEnumerable<Inmueble>> GetLastInmueblesAsync(int idMunicipio);
        Task<IEnumerable<Inmueble>> SearchInmueblesAsync(string nombre);
    }

}
