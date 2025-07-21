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
        private readonly ITributoService _tributoService;

        public TipoImpuestoController(IBaseService<TipoImpuesto> baseService, ITributoService tributoService, ILogger<TipoImpuestoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
            _tributoService = tributoService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TipoImpuesto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TipoImpuesto>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los tipos de impuesto");

            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<TipoImpuesto>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.MunicipioId == idMunicipio);

            var resultadoMapeado = _mapper.Map<IEnumerable<TipoImpuesto>>(filtrados);

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

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<TipoImpuesto>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var entity = _mapper.Map<TipoImpuesto>(dto);

            entity.MunicipioId = idMunicipio;

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

        [HttpPost("ApplyToAll/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<TipoImpuesto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> ApplyToAll(int id)
        {
            _logger.LogInformation($"Aplicando tipo de impuesto con ID {id} a todos los contribuyentes");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tipo de impuesto con ID {id} para aplicar"));

            var aplicado = await _tributoService.ApplyToAll(existingEntity, idMunicipio);

            if (!aplicado)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo aplicar el tipo de impuesto con ID {id} a todos los contribuyentes"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tipo de impuesto aplicado a todos los contribuyentes correctamente");

            return Ok(resultadoDTO);
        }
    }

}
