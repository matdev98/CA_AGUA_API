using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<T?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<T> AddAsync(T entity) => await _repository.AddAsync(entity);
        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public async Task<bool> UpdateAsync(int id, T entity) => await _repository.UpdateAsync(id, entity);

        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}
