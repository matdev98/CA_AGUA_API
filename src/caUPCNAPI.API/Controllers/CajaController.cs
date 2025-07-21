using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly ILogger<CajaController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<InicioCaja> _inicioBaseServices;
        private readonly IBaseService<CierreCaja> _baseService;
        private readonly ICierreCajaService _cierrecajaService;
        private readonly IInicioCajaService _inicioCajaService;

        public CajaController(IBaseService<CierreCaja> baseService, IBaseService<InicioCaja> iniciobaseService, ILogger<CajaController> logger, ICierreCajaService cierrecajaService, IInicioCajaService inicioCajaService , IMapper mapper)
        {
            _baseService = baseService;
            _inicioBaseServices = inicioCajaService;
            _cierrecajaService = cierrecajaService;
            _inicioCajaService = inicioCajaService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("CierresCaja")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<CierreCaja>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<CierreCaja>>>> GetAllCierreCaja([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta)
        {
            _logger.LogInformation("Obteniendo todos los cierres de caja");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<CierreCaja>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var cierres = await _cierrecajaService.ObtenerCierreCajaPeriodoAsync(idMunicipio, fechaDesde, fechaHasta);
            var resultadoDTO = ResultadoDTO<IEnumerable<CierreCaja>>.Exitoso(cierres, "Cierres obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("CierreCaja/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<CierreCaja>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<CierreCaja>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo cierre de caja con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<CierreCaja>.Fallido($"No se encontró el cierre de caja con ID {id}"));

            var resultadoMapeado = _mapper.Map<CierreCaja>(resultado);
            var resultadoDTO = ResultadoDTO<CierreCaja>.Exitoso(resultadoMapeado, "Cierre de caja encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost("CierreCaja")]
        [ProducesResponseType(typeof(ResultadoDTO<CierreCaja>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<CierreCaja>>> Create()
        {
            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (idUsuario <= 0)
            {
                return BadRequest("El ID de usuario y el ID de municipio son obligatorios y deben ser mayores a 0.");
            }

            try
            {
                _logger.LogInformation($"Controlador: Recibida solicitud de procesamiento de cierre de caja para Usuario: {idUsuario}.");

                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<MedioPago>>.Fallido("El Token no contiene IdMunicipio"));
                }
                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                var nuevoCierre = await _cierrecajaService.ProcesarCierreDeCajaAsync(idUsuario, idMunicipio);

                if (nuevoCierre == null)
                {
                    _logger.LogInformation("Controlador: No se generó un nuevo cierre de caja, probablemente por ausencia de pagos pendientes.");
                    return Ok(new { Message = "No se encontraron pagos pendientes de cierre para este período. No se generó un nuevo cierre de caja." });
                }

                _logger.LogInformation($"Controlador: Cierre de caja procesado y registrado con éxito. ID: {nuevoCierre.Id}.");
                return Ok(nuevoCierre); // Devuelve el objeto CierreCaja completo
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, $"Controlador: Error en la aplicación al procesar el cierre de caja.");
                return StatusCode(500, $"Error interno al procesar el cierre de caja: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controlador: Ocurrió un error inesperado al procesar el cierre de caja.");
                return StatusCode(500, "Ocurrió un error inesperado al procesar el cierre de caja.");
            }
            
        }

        [HttpPut("CierreCaja/anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateCierreCajaEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del cierre de caja {id} a 2 (Inactivo).");

            try
            {
                // Llama al método del servicio específico del contribuyente
                bool success = await _cierrecajaService.UpdateCierreCajaEstadoIdAsync(id);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"Cierre de caja {id} actualizado a (Inactivo) correctamente.");
                    return Ok(resultadoDTO);
                }
                else
                {
                    _logger.LogWarning($"No se pudo actualizar el EstadoId del cierre de caja {id}. Es posible que no exista.");
                    return NotFound(ResultadoDTO<bool>.Fallido($"Cierre de Caja con Id: {id} no encontrado o no se pudo actualizar el estado."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el EstadoId del contribuyente {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<bool>.Fallido("Error interno del servidor al actualizar el estado del contribuyente."));
            }
        }

        [HttpGet("PagosCerrados")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>>> GetPagosCerrados([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta)
        {
            _logger.LogInformation("Obteniendo pagos cerrados");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var pagosCerrados = await _cierrecajaService.ObtenerPagosCerradosAsync(idMunicipio, fechaDesde, fechaHasta);
            var resultadoDTO = ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Exitoso(pagosCerrados, "Pagos cerrados obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("PagosSinCerrar")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>>> GetPagosSinCerrar()
        {
            _logger.LogInformation("Obteniendo pagos sin cerrar");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var pagosSinCerrar = await _cierrecajaService.ObtenerPagosSinCerrarAsync(idMunicipio);
            var resultadoDTO = ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Exitoso(pagosSinCerrar, "Pagos sin cerrar obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("PagosDeUnCierre/{idCierre}")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>>> GetPagosDeUnCierre(int idCierre)
        {
            _logger.LogInformation($"Obteniendo pagos de un cierre con ID {idCierre}");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var pagosDeUnCierre = await _cierrecajaService.ObtenerPagosDeUnCierreAsync(idMunicipio, idCierre);
            var resultadoDTO = ResultadoDTO<IEnumerable<PagoCerradoDetalleDTO>>.Exitoso(pagosDeUnCierre, "Pagos de un cierre obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("InicioCaja")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<InicioCaja>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<InicioCaja>>>> GetAllInicioCaja([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta)
        {
            _logger.LogInformation("Obteniendo todos los inicios de caja");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<CierreCaja>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var inicio = await _inicioCajaService.ObtenerInicioCajaPeriodoAsync(idMunicipio, fechaDesde, fechaHasta);
            var resultadoDTO = ResultadoDTO<IEnumerable<InicioCaja>>.Exitoso(inicio, "Listado de inicios de caja obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("InicioCaja/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<InicioCaja>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<InicioCaja>>> GetInicioCajaById(int id)
        {
            _logger.LogInformation($"Obteniendo inicio de caja con ID {id}");

            var resultado = await _inicioBaseServices.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<InicioCaja>.Fallido($"No se encontró el inicio de caja con ID {id}"));

            var resultadoMapeado = _mapper.Map<InicioCaja>(resultado);
            var resultadoDTO = ResultadoDTO<InicioCaja>.Exitoso(resultadoMapeado, "Inicio de caja encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost("InicioCaja")]
        [ProducesResponseType(typeof(ResultadoDTO<InicioCaja>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<InicioCaja>>> CreateInicioCaja([FromBody] InicioCajaDTO dto)
        {
            if (dto == null || dto.Monto <= 0)
            {
                return BadRequest("El ID de usuario y el monto inicial son obligatorios y deben ser mayores a 0.");
            }

            try
            {

                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<InicioCaja>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (idUsuarioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<InicioCaja>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idUsuario = int.Parse(idUsuarioClaim.Value);

                var nuevoInicio = await _inicioCajaService.ProcesarInicioDeCajaAsync(idUsuario, idMunicipio, dto.Monto);

                if (nuevoInicio == null)
                {
                    _logger.LogInformation("Controlador: No se generó un nuevo cierre de caja, probablemente por ausencia de pagos pendientes.");
                    return Ok(new { Message = "No se encontraron pagos pendientes de cierre para este período. No se generó un nuevo cierre de caja." });
                }

                _logger.LogInformation($"Controlador: Cierre de caja procesado y registrado con éxito. ID: {nuevoInicio.Id}.");
                return Ok(nuevoInicio); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controlador: Error en la aplicación al procesar el inicio de caja.");
                return StatusCode(500, $"Error interno al procesar el cierre de caja: {ex.Message}");
            }
        }

        [HttpPut("InicioCaja/anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateInicioCajaEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del inicio de caja {id} a 2 (Inactivo).");

            try
            {
                // Llama al método del servicio específico del contribuyente
                bool success = await _inicioCajaService.UpdateInicioCajaEstadoIdAsync(id);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"Inicio de caja {id} actualizado a (Inactivo) correctamente.");
                    return Ok(resultadoDTO);
                }
                else
                {
                    _logger.LogWarning($"No se pudo actualizar el EstadoId del inicio de caja {id}. Es posible que no exista.");
                    return NotFound(ResultadoDTO<bool>.Fallido($"Inicio de Caja con Id: {id} no encontrado o no se pudo actualizar el estado."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el EstadoId del contribuyente {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<bool>.Fallido("Error interno del servidor al actualizar el estado del contribuyente."));
            }
        }

        [HttpGet("generar-pdf")] // Cambiado a "generar-pdf" y usa un verbo HTTP más apropiado
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))] // Retorna un PDF
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si no se encuentran datos
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerarCierreCajaPDF(int idCierre)
        {
            try
            {

                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<MedioPago>>.Fallido("El Token no contiene IdMunicipio"));
                }
                int idMunicipio = int.Parse(idMunicipioClaim.Value);
                
                _logger.LogInformation($"Generando PDF de cierre de caja para idMunicipio: {idMunicipio}, idCIerre: {idCierre}");

                var pagosDeUnCierre = await _cierrecajaService.ObtenerPagosDeUnCierreAsync(idMunicipio, idCierre);

                byte[] pdfBytes = await _cierrecajaService.GenerarCierreCajaPdf(idMunicipio, idCierre, pagosDeUnCierre);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning($"No se pudo generar el pdf de cierre de caja o no se encontraron datos para idMunicipio: {idMunicipio}, idCierre: {idCierre}");
                    return NotFound("No se pudo generar el pdf de cierre de caja o no se encontraron datos."); // Código de estado 404
                }

                // Devolver el archivo PDF
                return File(pdfBytes, "application/pdf", $"cierrecaja_municipio_{idMunicipio}_cierre_{idCierre}.pdf"); // Buen nombre de archivo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar el PDF del cierre de caja: {idCierre}");
                return StatusCode(500, "Error al generar el PDF del cierre de caja."); // Código de estado 500 con mensaje
            }
        }
    }

}
