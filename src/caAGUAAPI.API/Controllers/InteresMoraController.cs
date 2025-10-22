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
    public class InteresMoraController : ControllerBase
    {
        private readonly ILogger<InteresMoraController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<InteresMora> _baseService;

        public InteresMoraController(IBaseService<InteresMora> baseService, ILogger<InteresMoraController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<InteresMora>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<InteresMora>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los registros de Intereses por Mora");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<InteresMora>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<InteresMora>>.Exitoso(resultadoMapeado, "Listado de registros de Intereses por Mora obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<InteresMora>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<InteresMora>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo Interes por Mora con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<InteresMora>.Fallido($"No se encontró los Intereses por Mora con ID {id}"));

            var resultadoMapeado = _mapper.Map<InteresMora>(resultado);
            var resultadoDTO = ResultadoDTO<InteresMora>.Exitoso(resultadoMapeado, "Intereses por Mora encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<InteresMora>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<InteresMora>>> Create([FromBody] InteresMoraDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Interes por Mora");

            var entity = _mapper.Map<InteresMora>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<InteresMora>(createdEntity);

            var resultadoDTO = ResultadoDTO<InteresMora>.Exitoso(resultadoMapeado, "Interes por Mora creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] InteresMoraDTO dto)
        {
            _logger.LogInformation($"Actualizando Interes por Mora con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Interes por Mora con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Interes por Mora con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Interes por Mora actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Interes por Mora con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Interes por Mora con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Interes por Mora eliminada correctamente");

            return Ok(resultadoDTO);
        }

    }

}
