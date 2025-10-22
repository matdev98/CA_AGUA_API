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
    public class TributoInmobiliarioController : ControllerBase
    {
        private readonly ILogger<TributoInmobiliarioController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<TributoInmobiliario> _baseService;

        public TributoInmobiliarioController(IBaseService<TributoInmobiliario> baseService, ILogger<TributoInmobiliarioController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TributoInmobiliario>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TributoInmobiliario>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los tributos inmobiliarios");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<TributoInmobiliario>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<TributoInmobiliario>>.Exitoso(resultadoMapeado, "Listado de tributos inmobiliarios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TributoInmobiliario>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<TributoInmobiliario>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tributo inmobiliario con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<TributoInmobiliario>.Fallido($"No se encontró el tributo inmobiliario con ID {id}"));

            var resultadoMapeado = _mapper.Map<TributoInmobiliario>(resultado);
            var resultadoDTO = ResultadoDTO<TributoInmobiliario>.Exitoso(resultadoMapeado, "Tributo inmobiliario encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<TributoInmobiliario>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<TributoInmobiliario>>> Create([FromBody] TributoInmobiliarioDTO dto)
        {
            _logger.LogInformation("Creando un nuevo tributo inmobiliario");

            var entity = _mapper.Map<TributoInmobiliario>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<TributoInmobiliario>(createdEntity);

            var resultadoDTO = ResultadoDTO<TributoInmobiliario>.Exitoso(resultadoMapeado, "Tributo inmobiliario creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdTributo }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TributoInmobiliarioDTO dto)
        {
            _logger.LogInformation($"Actualizando tributo inmobiliario con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tributo inmobiliario con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el tributo inmobiliario con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tributo inmobiliario actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando tributo inmobiliario con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tributo inmobiliario con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tributo inmobiliario eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
