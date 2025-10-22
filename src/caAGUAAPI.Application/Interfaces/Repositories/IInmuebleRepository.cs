using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Interfaces.Repositories
{
    public interface IInmuebleRepository : IBaseRepository<Inmueble>
    {
        Task<IEnumerable<Inmueble>> GetByMunicipioIdAsync(int idMunicipio);
        Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId);
        Task<bool> UpdateEstadoIdAsync(int id, int idUsuario);
        Task<IEnumerable<Inmueble>> GetLastAddedInmueblesAsync(int idMunicipio);
        Task<IEnumerable<Inmueble>> SearchInmueblesByNameAsync(string nombre);
    }
}
