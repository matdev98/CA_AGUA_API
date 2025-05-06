using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IMapper _mapper;

        private readonly IUserService _userService;
        private readonly IBaseService<Usuarios> _baseService;         
        

        public UsuariosController(IUserService userService, IBaseService<Usuarios> baseService , ILogger<UsuariosController> logger, IMapper mapper)
        {
            _userService = userService;
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        ///// BASE REPOSITORY PRUEBA
        /// <summary>
        /// BaseService
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<UserDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<UserDTO>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los Usuarios");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<UserDTO>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<UserDTO>>.Exitoso(resultadoMapeado, "Listado de usuarios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<UserDTO>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo usuario con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<UserDTO>.Fallido($"No se encontró el usuario con ID {id}"));

            var resultadoMapeado = _mapper.Map<UserDTO>(resultado);
            var resultadoDTO = ResultadoDTO<UserDTO>.Exitoso(resultadoMapeado, "Usuario encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<UserDTO>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<UserDTO>>> Create([FromBody] UserCreateDTO dto)
        {
            _logger.LogInformation("Creando un nuevo usuario");

            var entity = _mapper.Map<Usuarios>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<UserDTO>(createdEntity);

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

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el usuario con ID {id}"));

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
