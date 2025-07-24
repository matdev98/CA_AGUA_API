using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IPermisoRepository
    {
        Task<Permiso?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Permiso dto);
        Task<bool> DeleteAsync(int id);
    }
}
