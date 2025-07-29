using caMUNICIPIOSAPI.Application.DTOs;
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

        public async Task<Rol> GetRolByIdUsuario(int id)
        {
            var idRol = await _context.UsuariosRoles
                                    .Where(ur => ur.IdUsuario == id)
                                    .Select(ur => ur.IdRol)
                                    .FirstOrDefaultAsync();
            if (idRol == 0)
            {
                return null;
            } 

            var rol = await _context.Roles
                                    .Where(r => r.IdRol == idRol)
                                    .FirstOrDefaultAsync();
            return rol;
        }

        public async Task<string> GetNombreRol(int idUsuario)
        {
            var idRol = await _context.UsuariosRoles
                                    .Where(ur => ur.IdUsuario == idUsuario)
                                    .Select(ur => ur.IdRol)
                                    .FirstOrDefaultAsync();
            if (idRol == 0)
            {
                return null;
            }

            var nombreRol = await _context.Roles
                                    .Where(r => r.IdRol == idRol)
                                    .Select(r => r.NombreRol)
                                    .FirstOrDefaultAsync();

            return nombreRol.ToUpper();
        }

        public async Task<List<UserDTO>> GetUsersByMunicipioAsync(int idMunicipio)
        {
            return await _context.Usuarios
                                 .Where(u => u.IdMunicipio == idMunicipio)
                                 .Select(u => new UserDTO
                                 {
                                     Id = u.Id,
                                     NombreUsuario = u.NombreUsuario,
                                     Email = u.Email,
                                     NombreCompleto = u.NombreCompleto,
                                     Activo = u.Activo,
                                     IdMunicipio = u.IdMunicipio
                                 })
                                 .ToListAsync();
        }

        public async Task<bool> CheckUsername(string username, string email)
        {
            return await _context.Usuarios
                            .Where(u => u.NombreUsuario == username || u.Email == email)
                            .AnyAsync();
        }

        public async Task<bool> CheckUsername(string username, string email, int id)
        {
            return await _context.Usuarios
                            .Where(u => (u.NombreUsuario == username || u.Email == email) && u.Id != id)
                            .AnyAsync();
        }
    }
}
