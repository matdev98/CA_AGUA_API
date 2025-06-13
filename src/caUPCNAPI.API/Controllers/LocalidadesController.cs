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
    public class LocalidadesController : ControllerBase
    {
        private readonly ILogger<LocalidadesController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Localidad> _baseService;

        public LocalidadesController(IBaseService<Localidad> baseService, ILogger<LocalidadesController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Localidad>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Localidad>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos las localidades");

            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Localidad>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.IdMunicipio == idMunicipio);

            var resultadoMapeado = _mapper.Map<IEnumerable<Localidad>>(filtrados);

            var resultadoDTO = ResultadoDTO<IEnumerable<Localidad>>.Exitoso(resultadoMapeado, "Listado de localidades obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Localidad>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Localidad>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo localidades con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Localidad>.Fallido($"No se encontró la localidad con ID {id}"));

            var resultadoMapeado = _mapper.Map<Localidad>(resultado);
            var resultadoDTO = ResultadoDTO<Localidad>.Exitoso(resultadoMapeado, "Localidad encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Localidad>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Localidad>>> Create([FromBody] LocalidadDTO dto)
        {
            _logger.LogInformation("Creando una nueva localidad");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Localidad>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var entity = _mapper.Map<Localidad>(dto);

            entity.IdMunicipio = idMunicipio;

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Localidad>(createdEntity);

            var resultadoDTO = ResultadoDTO<Localidad>.Exitoso(resultadoMapeado, "Localidad creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] LocalidadDTO dto)
        {
            _logger.LogInformation($"Actualizando localidad con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la localidad con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la localidad con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Localidad actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando localidad con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la localidad con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Localidad eliminada correctamente");

            return Ok(resultadoDTO);
        }

    }

}
