using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IntegracionExternaController : ControllerBase
    {
        private readonly ILogger<IntegracionExternaController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<IntegracionExterna> _baseService;

        public IntegracionExternaController(IBaseService<IntegracionExterna> baseService, ILogger<IntegracionExternaController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<IntegracionExterna>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<IntegracionExterna>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos las auditorias");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<IntegracionExterna>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<IntegracionExterna>>.Exitoso(resultadoMapeado, "Listado de auditorias obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<IntegracionExterna>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IntegracionExterna>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo auditorias con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<IntegracionExterna>.Fallido($"No se encontró la auditoria con ID {id}"));

            var resultadoMapeado = _mapper.Map<IntegracionExterna>(resultado);
            var resultadoDTO = ResultadoDTO<IntegracionExterna>.Exitoso(resultadoMapeado, "Auditoria encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<IntegracionExterna>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<IntegracionExterna>>> Create([FromBody] IntegracionExternaDTO dto)
        {
            _logger.LogInformation("Creando una nueva auditoria");

            var entity = _mapper.Map<IntegracionExterna>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<IntegracionExterna>(createdEntity);

            var resultadoDTO = ResultadoDTO<IntegracionExterna>.Exitoso(resultadoMapeado, "Auditoria creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdIntegracion }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] IntegracionExternaDTO dto)
        {
            _logger.LogInformation($"Actualizando auditoria con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la auditoria con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la auditoria con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Auditoria actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Auditoria con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la Auditoria con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Auditoria eliminada correctamente");

            return Ok(resultadoDTO);
        }

    }

}
