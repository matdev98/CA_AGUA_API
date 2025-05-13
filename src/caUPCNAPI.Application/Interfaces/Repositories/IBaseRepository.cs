using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> UpdateAsync(int id, T entity);
        Task<bool> DeleteAsync(int id);
    }
}
