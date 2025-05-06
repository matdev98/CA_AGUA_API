using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuarioRolController : ControllerBase
    {
        private readonly ILogger<UsuarioRolController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<UsuarioRol> _baseService;

        public UsuarioRolController(IBaseService<UsuarioRol> baseService, ILogger<UsuarioRolController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<UsuarioRol>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<UsuarioRol>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los Usuario Rol");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<UsuarioRol>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<UsuarioRol>>.Exitoso(resultadoMapeado, "Listado de Usuario Rol obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<UsuarioRol>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<UsuarioRol>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo Usuario Rol con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<UsuarioRol>.Fallido($"No se encontró el Usuario Rol con ID {id}"));

            var resultadoMapeado = _mapper.Map<UsuarioRol>(resultado);
            var resultadoDTO = ResultadoDTO<UsuarioRol>.Exitoso(resultadoMapeado, "Usuario Rol encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<UsuarioRol>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<UsuarioRol>>> Create([FromBody] UsuarioRolDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Usuario Rol");

            var entity = _mapper.Map<UsuarioRol>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<UsuarioRol>(createdEntity);

            var resultadoDTO = ResultadoDTO<UsuarioRol>.Exitoso(resultadoMapeado, "Usuario Rol creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { Id = createdEntity.IdRol }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] UsuarioRolDTO dto)
        {
            _logger.LogInformation($"Actualizando Usuario Rol con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Usuario Rol con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Usuario Rol con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Usuario Rol actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Usuario Rol con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Usuario Rol con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Usuario Rol eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
