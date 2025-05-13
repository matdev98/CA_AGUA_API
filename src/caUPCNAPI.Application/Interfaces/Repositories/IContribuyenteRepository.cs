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
        Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio);
    }
}
