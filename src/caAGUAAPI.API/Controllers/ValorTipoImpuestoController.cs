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
    public class ValorTipoImpuestoController : ControllerBase
    {
        private readonly IValorTipoImpuestoService _valorTipoImpuestoService;
        private readonly ILogger<ValorTipoImpuestoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<ValorTipoImpuesto> _baseService;

        public ValorTipoImpuestoController(IBaseService<ValorTipoImpuesto> baseService, IValorTipoImpuestoService valorTipoImpuestoService , ILogger<ValorTipoImpuestoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _valorTipoImpuestoService = valorTipoImpuestoService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet] // La ruta específica para este endpoint será /api/ValorTipoImpuesto/detalles-nombre
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<NombreTipoImpuestoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<NombreTipoImpuestoDTO>>>> GetNombresTiposImpuestoConDetalle()
        {
            _logger.LogInformation("Recibida solicitud para obtener nombres y detalles de valores de tipos de impuesto.");
            try
            {
                // Llama al método del servicio para obtener los datos
                var tiposImpuestoDetalle = await _valorTipoImpuestoService.GetNombreTipoImpuestoAsync();

                // Verifica si se encontraron resultados
                if (tiposImpuestoDetalle == null || !tiposImpuestoDetalle.Any())
                {
                    _logger.LogWarning("No se encontraron tipos de impuesto con detalles de valor.");
                    // Retorna un 404 Not Found si no hay datos
                    return NotFound(ResultadoDTO<IEnumerable<NombreTipoImpuestoDTO>>.Fallido("No se encontraron tipos de impuesto con detalles de valor."));
                }

                // Si se encontraron datos, retorna un 200 OK con los resultados
                var resultadoDTO = ResultadoDTO<IEnumerable<NombreTipoImpuestoDTO>>.Exitoso(tiposImpuestoDetalle, "Lista de tipos de impuesto con detalles de valor obtenida correctamente.");
                return Ok(resultadoDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el controlador al obtener los nombres y detalles de valores de tipos de impuesto.");
                // Retorna un 500 Internal Server Error si ocurre una excepción
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<IEnumerable<NombreTipoImpuestoDTO>>.Fallido("Error interno del servidor al obtener los tipos de impuesto."));
            }
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

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<ValorTipoImpuesto>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            entity.OpCrea = idUsuario;
            entity.FecCrea = DateTime.Now;

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

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            _mapper.Map(dto, existingEntity);

            existingEntity.OpMod = idUsuario;
            existingEntity.FecMod = DateTime.Now;

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el valor tipo impuesto con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valor tipo impuesto actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Anular(int id)
        {
            _logger.LogInformation($"Anulando valor tipo impuesto con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el valor tipo impuesto con ID {id} para anular"));

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);
            
            existingEntity.Anulado = true;
            existingEntity.OpAnula = idUsuario;
            existingEntity.FecAnula = DateTime.Now;

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo anular el valor tipo impuesto con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valor tipo impuesto anulado correctamente");

            return Ok(resultadoDTO);
        }

        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        //{
        //    _logger.LogInformation($"Eliminando valor tipo impuesto con ID {id}");

        //    var deleted = await _baseService.DeleteAsync(id);

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el valor tipo impuesto con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Valor tipo impuesto eliminado correctamente");

        //    return Ok(resultadoDTO);
        //}
    }

}
