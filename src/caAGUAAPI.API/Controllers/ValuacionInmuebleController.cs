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
    public class ValuacionInmuebleController : ControllerBase
    {
        private readonly ILogger<ValuacionInmuebleController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<ValuacionInmueble> _baseService;

        public ValuacionInmuebleController(IBaseService<ValuacionInmueble> baseService, ILogger<ValuacionInmuebleController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<ValuacionInmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<ValuacionInmueble>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las valuaciones de inmuebles");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<ValuacionInmueble>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<ValuacionInmueble>>.Exitoso(resultadoMapeado, "Listado de valuaciones de inmuebles obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<ValuacionInmueble>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<ValuacionInmueble>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo valuación de inmueble con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<ValuacionInmueble>.Fallido($"No se encontró la valuación de inmueble con ID {id}"));

            var resultadoMapeado = _mapper.Map<ValuacionInmueble>(resultado);
            var resultadoDTO = ResultadoDTO<ValuacionInmueble>.Exitoso(resultadoMapeado, "Valuación de inmueble encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<ValuacionInmueble>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<ValuacionInmueble>>> Create([FromBody] ValuacionInmuebleDTO dto)
        {
            _logger.LogInformation("Creando una nueva valuación de inmueble");

            var entity = _mapper.Map<ValuacionInmueble>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<ValuacionInmueble>(createdEntity);

            var resultadoDTO = ResultadoDTO<ValuacionInmueble>.Exitoso(resultadoMapeado, "Valuación de inmueble creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdValuacion }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ValuacionInmuebleDTO dto)
        {
            _logger.LogInformation($"Actualizando valuación de inmueble con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la valuación de inmueble con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la valuación de inmueble con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valuación de inmueble actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando valuación de inmueble con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la valuación de inmueble con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valuación de inmueble eliminada correctamente");

            return Ok(resultadoDTO);
        }
    }

}
