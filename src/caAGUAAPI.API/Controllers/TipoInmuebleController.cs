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
    public class TipoInmuebleController : ControllerBase
    {
        private readonly ILogger<TipoInmuebleController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<TipoInmueble> _baseService;

        public TipoInmuebleController(IBaseService<TipoInmueble> baseService, ILogger<TipoInmuebleController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TipoInmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TipoInmueble>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los tipos de inmueble");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<TipoInmueble>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<TipoInmueble>>.Exitoso(resultadoMapeado, "Listado de tipos de inmueble obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TipoInmueble>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<TipoInmueble>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tipo de inmueble con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<TipoInmueble>.Fallido($"No se encontró el tipo de inmueble con ID {id}"));

            var resultadoMapeado = _mapper.Map<TipoInmueble>(resultado);
            var resultadoDTO = ResultadoDTO<TipoInmueble>.Exitoso(resultadoMapeado, "Tipo de inmueble encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<TipoInmueble>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<TipoInmueble>>> Create([FromBody] TipoInmuebleDTO dto)
        {
            _logger.LogInformation("Creando un nuevo tipo de inmueble");

            var entity = _mapper.Map<TipoInmueble>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<TipoInmueble>(createdEntity);

            var resultadoDTO = ResultadoDTO<TipoInmueble>.Exitoso(resultadoMapeado, "Tipo de inmueble creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TipoInmuebleDTO dto)
        {
            _logger.LogInformation($"Actualizando tipo de inmueble con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de inmueble con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el tipo de inmueble con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de inmueble actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando tipo de inmueble con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de inmueble con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de inmueble eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
