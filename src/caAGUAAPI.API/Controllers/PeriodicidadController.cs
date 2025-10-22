using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caAGUAAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PeriodicidadController : ControllerBase
    {
        private readonly ILogger<PeriodicidadController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<Periodicidad> _baseService;

        public PeriodicidadController(IBaseService<Periodicidad> baseService, ILogger<PeriodicidadController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Periodicidad>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Periodicidad>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las periodicidades");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Periodicidad>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Periodicidad>>.Exitoso(resultadoMapeado, "Listado de periodicidades obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Periodicidad>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Periodicidad>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo periodicidad con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Periodicidad>.Fallido($"No se encontró la periodicidad con ID {id}"));

            var resultadoMapeado = _mapper.Map<Periodicidad>(resultado);
            var resultadoDTO = ResultadoDTO<Periodicidad>.Exitoso(resultadoMapeado, "Periodicidad encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Periodicidad>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Periodicidad>>> Create([FromBody] PeriodicidadDTO dto)
        {
            _logger.LogInformation("Creando una nueva periodicidad");

            var entity = _mapper.Map<Periodicidad>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Periodicidad>(createdEntity);

            var resultadoDTO = ResultadoDTO<Periodicidad>.Exitoso(resultadoMapeado, "Periodicidad creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdPeriodicidad }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PeriodicidadDTO dto)
        {
            _logger.LogInformation($"Actualizando periodicidad con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la periodicidad con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la periodicidad con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Periodicidad actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando periodicidad con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la periodicidad con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Periodicidad eliminada correctamente");

            return Ok(resultadoDTO);
        }
    }

}
