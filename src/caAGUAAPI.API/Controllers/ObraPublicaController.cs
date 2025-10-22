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
    public class ObraPublicaController : ControllerBase
    {
        private readonly ILogger<ObraPublicaController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<ObraPublica> _baseService;

        public ObraPublicaController(IBaseService<ObraPublica> baseService, ILogger<ObraPublicaController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<ObraPublica>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<ObraPublica>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos las obras publicas");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<ObraPublica>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<ObraPublica>>.Exitoso(resultadoMapeado, "Listado de obras publicas obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<ObraPublica>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<ObraPublica>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo obra publica con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<ObraPublica>.Fallido($"No se encontró la obra publica con ID {id}"));

            var resultadoMapeado = _mapper.Map<ObraPublica>(resultado);
            var resultadoDTO = ResultadoDTO<ObraPublica>.Exitoso(resultadoMapeado, "Obra publica encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<ObraPublica>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<ObraPublica>>> Create([FromBody] ObraPublicaDTO dto)
        {
            _logger.LogInformation("Creando una nueva obra publica");

            var entity = _mapper.Map<ObraPublica>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<ObraPublica>(createdEntity);

            var resultadoDTO = ResultadoDTO<ObraPublica>.Exitoso(resultadoMapeado, "Obra publica creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdObra }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ObraPublicaDTO dto)
        {
            _logger.LogInformation($"Actualizando obra publica con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la obra publica con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la obra publica con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Obra publica actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Obra publica con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la Obra publica con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Obra publica eliminada correctamente");

            return Ok(resultadoDTO);
        }

    }

}
