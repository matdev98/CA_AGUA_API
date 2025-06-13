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
    public class PagoController : ControllerBase
    {
        private readonly ILogger<PagoController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Pago> _baseService;
        private readonly IPagoService _pagoService;

        public PagoController(IBaseService<Pago> baseService, IPagoService pagoService, ILogger<PagoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _pagoService = pagoService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Pago>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Pago>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los pagos");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Pago>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Pago>>.Exitoso(resultadoMapeado, "Listado de pagos obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Pago>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Pago>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo pago con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Pago>.Fallido($"No se encontró el pago con ID {id}"));

            var resultadoMapeado = _mapper.Map<Pago>(resultado);
            var resultadoDTO = ResultadoDTO<Pago>.Exitoso(resultadoMapeado, "Pago encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("por-inmueble/{idContribuyente}/{idInmueble}/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<List<Pago>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<List<Pago>>>> ObtenerPagosPorInmueble(int idContribuyente, int idInmueble, string periodo)
        {
            var pagos = await _pagoService.ObtenerPagosPorInmuebleAsync(idContribuyente, idInmueble, periodo);

            if (pagos == null || !pagos.Any())
            {
                return NotFound(new ResultadoDTO<List<Pago>>
                {
                    EsExitoso = false,
                    Errores = new List<string> { "No se encontraron pagos para los datos ingresados." },
                    Mensaje = "No hay pagos registrados."
                });
            }

            return Ok(new ResultadoDTO<List<Pago>>
            {
                EsExitoso = true,
                Datos = pagos,
                Mensaje = "Pagos obtenidos correctamente."
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Pago>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Pago>>> Create([FromBody] PagoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo pago");

            var entity = _mapper.Map<Pago>(dto);

            entity.EstadoId = 1;

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Pago>(createdEntity);

            var resultadoDTO = ResultadoDTO<Pago>.Exitoso(resultadoMapeado, "Pago creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdPago }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PagoDTO dto)
        {
            _logger.LogInformation($"Actualizando pago con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el pago con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el pago con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Pago actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")] // Ruta descriptiva
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateInmuebleEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del inmueble {id} a Inactivo.");

            try
            {

                // Llama al método del servicio específico del inmueble
                bool success = await _pagoService.UpdateInmuebleEstadoIdAsync(id);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"EstadoId del inmueble {id} actualizado a Inactivo correctamente.");
                    return Ok(resultadoDTO);
                }
                else
                {
                    _logger.LogWarning($"No se pudo actualizar el EstadoId del inmueble {id}. Es posible que no exista.");
                    return NotFound(ResultadoDTO<bool>.Fallido($"Inmueble con Id: {id} no encontrado o no se pudo actualizar el estado."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el EstadoId del inmueble {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<bool>.Fallido("Error interno del servidor al actualizar el estado del inmueble."));
            }
        }

        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        //{
        //    _logger.LogInformation($"Eliminando pago con ID {id}");

        //    // Esta llamada ahora debería ir al DeleteAsync de PagoService,
        //    // que a su vez usa PagoRepository para buscar por IdPago
        //    var deleted = await _pagoService.DeleteAsync(id); // Si _baseService es IPagoService, perfecto

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el pago con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Pago eliminada correctamente");

        //    return Ok(resultadoDTO);
        //}

        [HttpGet("detalle-por-fechas")]
        [ProducesResponseType(typeof(ResultadoDTO<List<PagoDetalleDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResultadoDTO<List<PagoDetalleDTO>>>> GetPagosDetalle(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            _logger.LogInformation($"Consultando detalle de pagos para Fechas: {fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}, Municipio: {idMunicipio}");

            if (fechaInicio > fechaFin)
            {
                return BadRequest(ResultadoDTO<string>.Fallido("La fecha de inicio no puede ser posterior a la fecha de fin."));
            }

            try
            {
                var pagosDetalle = await _pagoService.GetPagosDetallePorFechasYMunicipioAsync(fechaInicio, fechaFin, idMunicipio);

                if (pagosDetalle == null || !pagosDetalle.Any())
                {
                    return NotFound(ResultadoDTO<string>.Fallido("No se encontraron pagos con los criterios especificados."));
                }

                var resultadoDTO = ResultadoDTO<List<PagoDetalleDTO>>.Exitoso(pagosDetalle, "Detalle de pagos obtenido correctamente.");
                return Ok(resultadoDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de pagos por fechas y municipio.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResultadoDTO<string>.Fallido($"Error interno del servidor: {ex.Message}"));
            }
        }

    }

}
