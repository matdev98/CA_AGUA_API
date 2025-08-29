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
    public class CierreCajaController : ControllerBase
    {
        private readonly ILogger<CierreCajaController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<CierreCaja> _baseService;
        private readonly IPagoService _pagoService;

        public CierreCajaController(IBaseService<CierreCaja> baseService, ILogger<CierreCajaController> logger, IPagoService pagoService, IMapper mapper)
        {
            _baseService = baseService;
            _pagoService = pagoService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<CierreCaja>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<CierreCaja>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los cierres de caja");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<CierreCaja>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<CierreCaja>>.Exitoso(resultadoMapeado, "Listado de cierres de caja obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
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

                var nuevoCierre = await _pagoService.ProcesarCierreDeCajaAsync(idUsuario, idMunicipio);

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
                _logger.LogError(ex, $"Controlador: Error en la aplicación al procesar el cierre de caja para Usuario: {idUsuario}.");
                return StatusCode(500, $"Error interno al procesar el cierre de caja: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controlador: Ocurrió un error inesperado al procesar el cierre de caja para Usuario: {idUsuario}.");
                return StatusCode(500, "Ocurrió un error inesperado al procesar el cierre de caja.");
            }
            
        }

        [HttpPut("AnularCierre/{idCierre}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Anular(int idCierre)
        {
            if (idCierre == 0)
                return NotFound(ResultadoDTO<string>.Fallido("El valor no puede ser 0"));

            _logger.LogInformation($"Anulando cierre de caja con ID {idCierre}");

            var cierreExistente = await _baseService.GetByIdAsync(idCierre);

            if (cierreExistente == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el cierre de caja con ID {idCierre} para anular"));

            try
            {
                var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (idUsuarioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
                }
                var idUsuario = int.Parse(idUsuarioClaim.Value);

                var anularCierre = await _pagoService.AnularCierreCajaAsync(idCierre, idUsuario);

                if (!anularCierre)
                {
                    return BadRequest(ResultadoDTO<string>.Fallido($"No se pudo anular el cierre de caja con ID {idCierre}"));
                }
                _logger.LogInformation($"Cierre de caja con ID {idCierre} anulado correctamente.");
                return Ok(ResultadoDTO<string>.Exitoso("Cierre de caja anulado correctamente", "Cierre de caja anulado correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al anular el cierre de caja con ID {idCierre}");
                return StatusCode(500, ResultadoDTO<string>.Fallido("Ocurrió un error al anular el cierre de caja"));
            }
        }


    }

}
