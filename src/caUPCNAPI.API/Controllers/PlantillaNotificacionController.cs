using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlantillaNotificacionController : ControllerBase
    {
        private readonly ILogger<PlantillaNotificacionController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<PlantillaNotificacion> _baseService;

        public PlantillaNotificacionController(IBaseService<PlantillaNotificacion> baseService, ILogger<PlantillaNotificacionController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<PlantillaNotificacion>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<PlantillaNotificacion>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las plantillas de notificación");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<PlantillaNotificacion>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<PlantillaNotificacion>>.Exitoso(resultadoMapeado, "Listado de plantillas de notificación obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<PlantillaNotificacion>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<PlantillaNotificacion>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo plantilla de notificación con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<PlantillaNotificacion>.Fallido($"No se encontró la plantilla de notificación con ID {id}"));

            var resultadoMapeado = _mapper.Map<PlantillaNotificacion>(resultado);
            var resultadoDTO = ResultadoDTO<PlantillaNotificacion>.Exitoso(resultadoMapeado, "Plantilla de notificación encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<PlantillaNotificacion>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<PlantillaNotificacion>>> Create([FromBody] PlantillaNotificacionDTO dto)
        {
            _logger.LogInformation("Creando una nueva plantilla de notificación");

            var entity = _mapper.Map<PlantillaNotificacion>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<PlantillaNotificacion>(createdEntity);

            var resultadoDTO = ResultadoDTO<PlantillaNotificacion>.Exitoso(resultadoMapeado, "Plantilla de notificación creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PlantillaNotificacionDTO dto)
        {
            _logger.LogInformation($"Actualizando plantilla de notificación con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la plantilla de notificación con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la plantilla de notificación con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Plantilla de notificación actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando plantilla de notificación con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la plantilla de notificación con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Plantilla de notificación eliminada correctamente");

            return Ok(resultadoDTO);
        }
    }

}
