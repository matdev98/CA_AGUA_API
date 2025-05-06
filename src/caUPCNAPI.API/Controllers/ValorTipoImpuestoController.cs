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
    public class ValorTipoImpuestoController : ControllerBase
    {
        private readonly ILogger<ValorTipoImpuestoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<ValorTipoImpuesto> _baseService;

        public ValorTipoImpuestoController(IBaseService<ValorTipoImpuesto> baseService, ILogger<ValorTipoImpuestoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<ValorTipoImpuesto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<ValorTipoImpuesto>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los valores tipo impuesto");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<ValorTipoImpuesto>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<ValorTipoImpuesto>>.Exitoso(resultadoMapeado, "Listado de valores tipo impuesto obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<ValorTipoImpuesto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<ValorTipoImpuesto>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo valor tipo impuesto con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<ValorTipoImpuesto>.Fallido($"No se encontró el valor tipo impuesto con ID {id}"));

            var resultadoMapeado = _mapper.Map<ValorTipoImpuesto>(resultado);
            var resultadoDTO = ResultadoDTO<ValorTipoImpuesto>.Exitoso(resultadoMapeado, "Valor tipo impuesto encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<ValorTipoImpuesto>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<ValorTipoImpuesto>>> Create([FromBody] ValorTipoImpuestoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo valor tipo impuesto");

            var entity = _mapper.Map<ValorTipoImpuesto>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<ValorTipoImpuesto>(createdEntity);

            var resultadoDTO = ResultadoDTO<ValorTipoImpuesto>.Exitoso(resultadoMapeado, "Valor tipo impuesto creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ValorTipoImpuestoDTO dto)
        {
            _logger.LogInformation($"Actualizando valor tipo impuesto con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el valor tipo impuesto con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el valor tipo impuesto con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valor tipo impuesto actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando valor tipo impuesto con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el valor tipo impuesto con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valor tipo impuesto eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
