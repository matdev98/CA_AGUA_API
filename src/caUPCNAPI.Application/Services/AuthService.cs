using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IPasswordService passwordService, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        public async Task<Usuarios> ValidarCredencialesAsync(string usuario, string clave)
        {
            var usuarioExistente = await _authRepository.ObtenerPorNombreUsuarioAsync(usuario);

            if (usuarioExistente == null || !_passwordService.VerifyPassword(clave, usuarioExistente.ClaveHash))
                return null;

            return usuarioExistente;
        }

        public async Task<bool> RegistrarUsuarioAsync(RegistroRequest registro)
        {
            var usuarioExistente = await _authRepository.ObtenerPorNombreUsuarioAsync(registro.NombreUsuario);
            if (usuarioExistente != null)
                return false;

            var nuevoUsuario = new Usuarios
            {
                NombreUsuario = registro.NombreUsuario,
                Email = registro.Email,
                ClaveHash = _passwordService.HashPassword(registro.Clave),
                NombreCompleto = registro.NombreCompleto,
                Activo = true,
                IdMunicipio = registro.IdMunicipio
            };

            await _authRepository.CrearUsuarioAsync(nuevoUsuario);
            return true;
        }

        public string GenerarToken(Usuarios usuario)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim("IdMunicipio", usuario.IdMunicipio.ToString()),
            new Claim("NombreCompleto", usuario.NombreCompleto)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string ObtenerRolesUsuario(int usuarioId)
        {
            var roles = _authRepository.ObtenerRolesUsuario(usuarioId);
            return roles;
        }

        public List<string> ObtenerPermisosRol(int usuarioId)
        {
            var permisos = _authRepository.ObtenerPermisosRol(usuarioId);
            return permisos;
        }
    }

}
