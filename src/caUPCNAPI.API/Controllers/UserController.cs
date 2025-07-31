using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IPasswordService _password;
        private readonly IMapper _mapper;

        private readonly IUserService _userService;
        private readonly IBaseService<Usuarios> _baseService;        
        private readonly IBaseService<UsuarioRol> _baseUserRolService;
        

        public UsuariosController(IUserService userService, IBaseService<Usuarios> baseService, IBaseService<UsuarioRol> baseUserRolService, 
            ILogger<UsuariosController> logger, IPasswordService password, IMapper mapper)
        {
            _userService = userService;
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
            _baseUserRolService = baseUserRolService;
            _password = password;
        }

        ///// BASE REPOSITORY PRUEBA
        /// <summary>
        /// BaseService
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<UserConRolDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<UserConRolDTO>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los Usuarios");

            var resultado = await _baseService.GetAllAsync();

            var resultadoMapeado = new List<UserConRolDTO>();

            foreach(var x in resultado)
            {
                var componeMapeado = new UserConRolDTO
                {
                    Id = x.Id,
                    NombreUsuario = x.NombreUsuario,
                    Email = x.Email,
                    NombreCompleto = x.NombreCompleto,
                    Activo = x.Activo,
                    IdMunicipio = x.IdMunicipio,
                    Rol = await _userService.GetNombreRol(x.Id)
                };

                resultadoMapeado.Add(componeMapeado);
            }

            var resultadoDTO = ResultadoDTO<IEnumerable<UserConRolDTO>>.Exitoso(resultadoMapeado, "Listado de usuarios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("PorMunicipio")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<UserConRolDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<UserConRolDTO>>>> GetUsersMunicipio()
        {
            _logger.LogInformation("Obteniendo todos los Usuarios del municipio");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }
            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _userService.GetUsersMunicipio(idMunicipio);

            var resultadoMapeado = new List<UserConRolDTO>();

            foreach (var x in resultado)
            {
                var componeMapeado = new UserConRolDTO
                {
                    Id = x.Id,
                    NombreUsuario = x.NombreUsuario,
                    Email = x.Email,
                    NombreCompleto = x.NombreCompleto,
                    Activo = x.Activo,
                    IdMunicipio = x.IdMunicipio,
                    Rol = await _userService.GetNombreRol(x.Id)
                };

                resultadoMapeado.Add(componeMapeado);
            }

            var resultadoDTO = ResultadoDTO<IEnumerable<UserConRolDTO>>.Exitoso(resultadoMapeado, "Listado de usuarios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<UserConRolDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<UserConRolDTO>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo usuario con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<UserConRolDTO>.Fallido($"No se encontró el usuario con ID {id}"));

            var resultadoMapeado = new UserConRolDTO
            {
                Id = resultado.Id,
                NombreUsuario = resultado.NombreUsuario,
                Email = resultado.Email,
                NombreCompleto = resultado.NombreCompleto,
                Activo = resultado.Activo,
                IdMunicipio = resultado.IdMunicipio,
                Rol = await _userService.GetNombreRol(id)
            };
            var resultadoDTO = ResultadoDTO<UserConRolDTO>.Exitoso(resultadoMapeado, "Usuario encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<UserDTO>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<UserDTO>>> Create([FromBody] UserCreateDTO dto)
        {
            _logger.LogInformation("Creando un nuevo usuario");

            var entity = new Usuarios
            {
                NombreUsuario = dto.NombreUsuario,
                Email = dto.Email,
                ClaveHash = _password.HashPassword(dto.ClaveHash),
                NombreCompleto = dto.NombreCompleto,
                Activo = dto.Activo ?? true,
                IdMunicipio = dto.idMunicipio
            };
            var existente = await _userService.CheckUsername(entity.NombreUsuario, entity.Email);
            if (existente)
            {
                return BadRequest(ResultadoDTO<UserDTO>.Fallido("Ya existe un usuario con el mismo nombre de usuario o email."));
            }
            var createdEntity = await _userService.CreateUser(entity);
            var resultadoMapeado = _mapper.Map<UserDTO>(createdEntity);

            string idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUsuario != null)
            {
                int id = int.Parse(idUsuario);

                var usuarioRol = new UsuarioRol
                {
                    IdUsuario = createdEntity.Id,
                    IdRol = dto.IdRol,
                    OpCrea = id
                };

                usuarioRol = await _baseUserRolService.AddAsync(usuarioRol);
            }

            var resultadoDTO = ResultadoDTO<UserDTO>.Exitoso(resultadoMapeado, "Usuario creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] UserUpdateDTO dto)
        {
            _logger.LogInformation($"Actualizando usuario con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el usuario con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var existente = await _userService.CheckUsername(existingEntity.NombreUsuario, existingEntity.Email, id);
            if (existente)
            {
                return BadRequest(ResultadoDTO<string>.Fallido("Ya existe un usuario con el mismo nombre de usuario o email."));
            }
            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el usuario con ID {id}"));

            // Actualizar el rol del usuario si se proporciona
            if (dto.idRol != null)
            {
                string idUsuario = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idUser = int.Parse(idUsuario);
                var usuarioRol = await _userService.CambiarRol(id, (int)dto.idRol, idUser);
            }

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Usuario actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando usuario con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el usuario con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Usuario eliminado correctamente");

            return Ok(resultadoDTO);
        }



    }
}
