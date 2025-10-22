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
    public class TipoDocumentoController : ControllerBase
    {
        private readonly ILogger<TipoDocumentoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<TipoDocumento> _baseService;

        public TipoDocumentoController(IBaseService<TipoDocumento> baseService, ILogger<TipoDocumentoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TipoDocumento>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TipoDocumento>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los tipos de documento");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<TipoDocumento>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<TipoDocumento>>.Exitoso(resultadoMapeado, "Listado de tipos de documento obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TipoDocumento>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<TipoDocumento>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tipo de documento con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<TipoDocumento>.Fallido($"No se encontró el tipo de documento con ID {id}"));

            var resultadoMapeado = _mapper.Map<TipoDocumento>(resultado);
            var resultadoDTO = ResultadoDTO<TipoDocumento>.Exitoso(resultadoMapeado, "Tipo de documento encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<TipoDocumento>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<TipoDocumento>>> Create([FromBody] TipoDocumentoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo tipo de documento");

            var entity = _mapper.Map<TipoDocumento>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<TipoDocumento>(createdEntity);

            var resultadoDTO = ResultadoDTO<TipoDocumento>.Exitoso(resultadoMapeado, "Tipo de documento creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TipoDocumentoDTO dto)
        {
            _logger.LogInformation($"Actualizando tipo de documento con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de documento con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el tipo de documento con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de documento actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando tipo de documento con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de documento con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de documento eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
