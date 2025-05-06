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
    public class MedioPagoController : ControllerBase
    {
        private readonly ILogger<MedioPagoController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<MedioPago> _baseService;

        public MedioPagoController(IBaseService<MedioPago> baseService, ILogger<MedioPagoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<MedioPago>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<MedioPago>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los Medios de Pago");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<MedioPago>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<MedioPago>>.Exitoso(resultadoMapeado, "Listado de Medios de Pago obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<MedioPago>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<MedioPago>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo Medio de Pago con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<MedioPago>.Fallido($"No se encontró el Medio de Pago con ID {id}"));

            var resultadoMapeado = _mapper.Map<MedioPago>(resultado);
            var resultadoDTO = ResultadoDTO<MedioPago>.Exitoso(resultadoMapeado, "Medio de Pago encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<MedioPago>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<MedioPago>>> Create([FromBody] MedioPagoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Medio de Pago");

            var entity = _mapper.Map<MedioPago>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<MedioPago>(createdEntity);

            var resultadoDTO = ResultadoDTO<MedioPago>.Exitoso(resultadoMapeado, "Medio de Pago creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] MedioPagoDTO dto)
        {
            _logger.LogInformation($"Actualizando Medio de Pago con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Medio de Pago con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Medio de Pago con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Medio de Pago actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando Medio de Pago con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Medio de Pago con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Medio de Pago eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
