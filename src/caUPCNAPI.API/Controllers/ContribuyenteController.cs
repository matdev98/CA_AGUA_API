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
    public class ContribuyentesController : ControllerBase
    {
        private readonly ILogger<ContribuyentesController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Contribuyente> _baseService;

        public ContribuyentesController(IBaseService<Contribuyente> baseService, ILogger<ContribuyentesController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Contribuyente>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los contribuyentes");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Contribuyente>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(resultadoMapeado, "Listado de contribuyentes obtenido correctamente");

            return Ok(resultadoDTO);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo contribuyente con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Contribuyente>.Fallido($"No se encontró el contribuyente con ID {id}"));

            var resultadoMapeado = _mapper.Map<Contribuyente>(resultado);
            var resultadoDTO = ResultadoDTO<Contribuyente>.Exitoso(resultadoMapeado, "contribuyente encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> Create([FromBody] ContribuyenteDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Contribuyente");

            var entity = _mapper.Map<Contribuyente>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Contribuyente>(createdEntity);

            var resultadoDTO = ResultadoDTO<Contribuyente>.Exitoso(resultadoMapeado, "Contribuyente creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ContribuyenteDTO dto)
        {
            _logger.LogInformation($"Actualizando contribuyente con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Contribuyente con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Contribuyente con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Contribuyente actualizado correctamente");

            return Ok(resultadoDTO);
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Contribuyente con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Contribuyente con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Contribuyente eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
