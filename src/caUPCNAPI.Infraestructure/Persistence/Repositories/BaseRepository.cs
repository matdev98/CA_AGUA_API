using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _entities;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        } 
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }  
        public async Task<T> GetByIdAsync(int id)
        {
            return await _entities
                         .Where(e => EF.Property<int>(e, "Id") == id)
                         .FirstOrDefaultAsync();
        } 
        public async Task<T> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(int id, T entity)
        {
            var existing = await _entities
                                 .Where(e => EF.Property<int>(e, "Id") == id) 
                                 .FirstOrDefaultAsync(); 

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _entities
                                 .Where(e => EF.Property<int>(e, "Id") == id) 
                                 .FirstOrDefaultAsync(); 

            if (existing == null)
                return false;

            _entities.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
