using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuarios?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                                 .Where(u => u.Id == id)
                                 .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            return await _context.Usuarios
                                 .ToListAsync(); 
        }


        public async Task AddAsync(Usuarios user)
        {
            await _context.Usuarios.AddAsync(user); 
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Usuarios user)
        {
            var existingUser = await _context.Usuarios
                                             .Where(u => u.Id == user.Id)
                                             .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var user = await _context.Usuarios
                                     .Where(u => u.Id == id)
                                     .FirstOrDefaultAsync();

            if (user != null)
            {
                _context.Usuarios.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

    }
}
