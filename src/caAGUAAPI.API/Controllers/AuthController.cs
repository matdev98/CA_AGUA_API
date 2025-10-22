using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Application.Services;
using caAGUAAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caAGUAAPI.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResultadoDTO<object>>> Login([FromBody] LoginRequest login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultadoDTO<object>.Fallido("Datos inválidos."));

            var usuario = await _authService.ValidarCredencialesAsync(login.Usuario, login.Clave);

            if (usuario == null)
                return Unauthorized(ResultadoDTO<object>.Fallido("Credenciales inválidas."));

            var token = _authService.GenerarToken(usuario);

            var usuarioDTO = new UsuarioConTokenDTO
            {
                Token = token,
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                Activo = usuario.Activo,
                Roles = _authService.ObtenerRolesUsuario(usuario.Id),
                Permisos = _authService.ObtenerPermisosRol(usuario.Id)
            };

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(8)
            };

            Response.Cookies.Append("auth_token", token, cookieOptions);

            return Ok(new ResultadoDTO<UsuarioConTokenDTO>
            {
                EsExitoso = true,
                Mensaje = "Login exitoso",
                Datos = usuarioDTO
            });
        }

        [HttpPost("Registro")]
        public async Task<ActionResult<ResultadoDTO<object>>> Register([FromBody] RegistroRequest registro)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultadoDTO<object>.Fallido("Datos inválidos."));

            var exito = await _authService.RegistrarUsuarioAsync(registro);

            if (!exito)
                return Conflict(ResultadoDTO<object>.Fallido("El nombre de usuario ya está registrado."));

            return Ok(ResultadoDTO<object>.Exitoso(null, "Usuario registrado correctamente."));
        }
    }
}
