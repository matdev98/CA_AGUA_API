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
    
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MunicipiosController : ControllerBase
    {
        private readonly ILogger<MunicipiosController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Municipio> _baseService;

        public MunicipiosController(IBaseService<Municipio> baseService, ILogger<MunicipiosController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("list")] // Un endpoint más específico para la lista simplificada
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<MunicipioListDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<MunicipioListDTO>>), StatusCodes.Status404NotFound)] // Si no se encuentran datos
        public async Task<ActionResult<ResultadoDTO<IEnumerable<MunicipioListDTO>>>> GetMunicipiosList()
        {
            _logger.LogInformation("Obteniendo lista simplificada de Municipios (Id y Nombre)");

            // Obtener todos los municipios (asumiendo que GetAllAsync devuelve la entidad completa)
            var municipios = await _baseService.GetAllAsync();

            // Mapear de IEnumerable<Municipio> a IEnumerable<MunicipioListDTO>
            var municipiosListDTO = _mapper.Map<IEnumerable<MunicipioListDTO>>(municipios);

            var resultadoDTO = ResultadoDTO<IEnumerable<MunicipioListDTO>>.Exitoso(municipiosListDTO, "Lista de municipios (Id y Nombre) obtenida correctamente.");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Municipio>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Municipio>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los Municipios");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Municipio>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Municipio>>.Exitoso(resultadoMapeado, "Listado de municipios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpGet("por-municipio")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Municipio>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Municipio>>>> GetMunicipioActual()
        {
            _logger.LogInformation("Obteniendo todos los Municipios");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.Id == idMunicipio);

            var resultadoMapeado = _mapper.Map<IEnumerable<Municipio>>(filtrados);

            var resultadoDTO = ResultadoDTO<IEnumerable<Municipio>>.Exitoso(resultadoMapeado, "Listado de municipios obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Municipio>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Municipio>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo municipio con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Municipio>.Fallido($"No se encontró el municipio con ID {id}"));

            var resultadoMapeado = _mapper.Map<Municipio>(resultado);
            var resultadoDTO = ResultadoDTO<Municipio>.Exitoso(resultadoMapeado, "Municipio encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Municipio>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Municipio>>> Create([FromBody] MunicipioDTO dto)
        {
            _logger.LogInformation("Creando un nuevo municipio");

            var entity = _mapper.Map<Municipio>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Municipio>(createdEntity);

            var resultadoDTO = ResultadoDTO<Municipio>.Exitoso(resultadoMapeado, "Municipio creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] MunicipioDTO dto)
        {
            _logger.LogInformation($"Actualizando municipio con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el municipio con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el municipio con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "municipio actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando municipio con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el municipio con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "municipio eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
