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
    public class TitularidadInmuebleController : ControllerBase
    {
        private readonly ILogger<TitularidadInmuebleController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<TitularidadInmueble> _baseService;

        public TitularidadInmuebleController(IBaseService<TitularidadInmueble> baseService, ILogger<TitularidadInmuebleController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TitularidadInmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TitularidadInmueble>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las titularidades de inmuebles");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<TitularidadInmueble>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<TitularidadInmueble>>.Exitoso(resultadoMapeado, "Listado de titularidades de inmuebles obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TitularidadInmueble>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<TitularidadInmueble>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo titularidad de inmueble con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<TitularidadInmueble>.Fallido($"No se encontró la titularidad de inmueble con ID {id}"));

            var resultadoMapeado = _mapper.Map<TitularidadInmueble>(resultado);
            var resultadoDTO = ResultadoDTO<TitularidadInmueble>.Exitoso(resultadoMapeado, "Titularidad de inmueble encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<TitularidadInmueble>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<TitularidadInmueble>>> Create([FromBody] TitularidadInmuebleDTO dto)
        {
            _logger.LogInformation("Creando una nueva titularidad de inmueble");

            var entity = _mapper.Map<TitularidadInmueble>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<TitularidadInmueble>(createdEntity);

            var resultadoDTO = ResultadoDTO<TitularidadInmueble>.Exitoso(resultadoMapeado, "Titularidad de inmueble creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdTitularidad }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TitularidadInmuebleDTO dto)
        {
            _logger.LogInformation($"Actualizando titularidad de inmueble con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la titularidad de inmueble con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la titularidad de inmueble con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Titularidad de inmueble actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando titularidad de inmueble con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la titularidad de inmueble con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Titularidad de inmueble eliminada correctamente");

            return Ok(resultadoDTO);
        }
    }

}
