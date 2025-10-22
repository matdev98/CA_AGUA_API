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
    public class InmuebleObraController : ControllerBase
    {
        private readonly ILogger<InmuebleObraController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<InmuebleObra> _baseService;

        public InmuebleObraController(IBaseService<InmuebleObra> baseService, ILogger<InmuebleObraController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<InmuebleObra>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<InmuebleObra>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos las auditorias");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<InmuebleObra>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<InmuebleObra>>.Exitoso(resultadoMapeado, "Listado de auditorias obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<InmuebleObra>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<InmuebleObra>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo auditorias con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<InmuebleObra>.Fallido($"No se encontró la auditoria con ID {id}"));

            var resultadoMapeado = _mapper.Map<InmuebleObra>(resultado);
            var resultadoDTO = ResultadoDTO<InmuebleObra>.Exitoso(resultadoMapeado, "Auditoria encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<InmuebleObra>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<InmuebleObra>>> Create([FromBody] InmuebleObraDTO dto)
        {
            _logger.LogInformation("Creando una nueva auditoria");

            var entity = _mapper.Map<InmuebleObra>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<InmuebleObra>(createdEntity);

            var resultadoDTO = ResultadoDTO<InmuebleObra>.Exitoso(resultadoMapeado, "Auditoria creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] InmuebleObraDTO dto)
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
