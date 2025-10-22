using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Application.Services;
using caAGUAAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caAGUAAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PermisoController : ControllerBase
    {
        private readonly ILogger<PermisoController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Permiso> _baseService;
        private readonly IPermisoService _permisoService;

        public PermisoController(IBaseService<Permiso> baseService, IPermisoService permisoService ,ILogger<PermisoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _permisoService = permisoService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Permiso>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Permiso>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los permisos");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Permiso>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Permiso>>.Exitoso(resultadoMapeado, "Listado de permisos obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Permiso>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Permiso>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo permisos con ID {id}");

            var resultado = await _permisoService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Permiso>.Fallido($"No se encontró el permiso con ID {id}"));

            var resultadoMapeado = _mapper.Map<Permiso>(resultado);
            var resultadoDTO = ResultadoDTO<Permiso>.Exitoso(resultadoMapeado, "Permiso encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Permiso>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Permiso>>> Create([FromBody] PermisoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Permiso");

            var entity = _mapper.Map<Permiso>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Permiso>(createdEntity);

            var resultadoDTO = ResultadoDTO<Permiso>.Exitoso(resultadoMapeado, "Permiso creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdPermiso }, resultadoDTO);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PermisoDTO dto)
        {
            _logger.LogInformation($"Actualizando Permiso con ID {id}");

            var existingEntity = await _permisoService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el permiso con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _permisoService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el permiso con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Permiso actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Permiso con ID {id}");

            var deleted = await _permisoService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el permiso con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Permiso eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
