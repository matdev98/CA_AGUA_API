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
    public class TipoImpuestoController : ControllerBase
    {
        private readonly ILogger<TipoImpuestoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<TipoImpuesto> _baseService;

        public TipoImpuestoController(IBaseService<TipoImpuesto> baseService, ILogger<TipoImpuestoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TipoImpuesto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TipoImpuesto>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los tipos de impuesto");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<TipoImpuesto>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<TipoImpuesto>>.Exitoso(resultadoMapeado, "Listado de tipos de impuesto obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TipoImpuesto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<TipoImpuesto>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tipo de impuesto con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<TipoImpuesto>.Fallido($"No se encontró el tipo de impuesto con ID {id}"));

            var resultadoMapeado = _mapper.Map<TipoImpuesto>(resultado);
            var resultadoDTO = ResultadoDTO<TipoImpuesto>.Exitoso(resultadoMapeado, "Tipo de impuesto encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<TipoImpuesto>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<TipoImpuesto>>> Create([FromBody] TipoImpuestoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo tipo de impuesto");

            var entity = _mapper.Map<TipoImpuesto>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<TipoImpuesto>(createdEntity);

            var resultadoDTO = ResultadoDTO<TipoImpuesto>.Exitoso(resultadoMapeado, "Tipo de impuesto creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TipoImpuestoDTO dto)
        {
            _logger.LogInformation($"Actualizando tipo de impuesto con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de impuesto con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el tipo de impuesto con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de impuesto actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando tipo de impuesto con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de impuesto con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de impuesto eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
