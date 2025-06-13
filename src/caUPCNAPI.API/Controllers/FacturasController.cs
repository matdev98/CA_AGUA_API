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
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;
        private readonly ILogger<FacturasController> _logger;

        public FacturasController(IFacturaService facturaService, ILogger<FacturasController> logger)
        {
            _facturaService = facturaService;
            _logger = logger;
        }

        [HttpGet("generar-pdf")] // Cambiado a "generar-pdf" y usa un verbo HTTP más apropiado
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))] // Retorna un PDF
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si no se encuentran datos
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerarFacturaPdf(int idContribuyente, string periodo)
        {
            try
            {
                _logger.LogInformation($"Generando PDF de factura para idContribuyente: {idContribuyente}, Periodo: {periodo}");

                // Obtener el IdMunicipio desde el token
                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                byte[] pdfBytes = await _facturaService.GenerarFacturaPorContribuyentePdf(idContribuyente, periodo, idMunicipio);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning($"No se pudo generar la factura o no se encontraron datos para idContribuyente: {idContribuyente}, Periodo: {periodo}");
                    return NotFound("No se pudo generar la factura o no se encontraron datos."); // Código de estado 404
                }

                // Devolver el archivo PDF
                return File(pdfBytes, "application/pdf", $"factura_contribuyente_{idContribuyente}_periodo_{periodo}.pdf"); // Buen nombre de archivo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar el PDF de la factura para idContribuyente: {idContribuyente}, Periodo: {periodo}");
                return StatusCode(500, "Error al generar el PDF de la factura."); // Código de estado 500 con mensaje
            }
        }

        [HttpGet("generar-recibo")] // Endpoint para generar el recibo
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))] // Retorna un PDF
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si no se encuentran datos
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerarRecibo(int IdPago, int idContribuyente)
        {
            // Validar si los parámetros de la solicitud son válidos
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Controlador: Solicitud de generación de recibo inválida. Detalles: {ModelStateErrors}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {

                // Obtener el IdMunicipio desde el token
                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                _logger.LogInformation($"Controlador: Recibida solicitud para generar recibo PDF para Pago: {IdPago}, Contribuyente: {idContribuyente}, Municipio: {idMunicipio}.");


                // Llama al servicio de recibos para generar y guardar el PDF
                byte[] pdfBytes = await _facturaService.GenerarYGuardarReciboPDFAsync(IdPago, idContribuyente, idMunicipio);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning($"Controlador: El servicio no pudo generar el PDF para Pago: {IdPago}. Esto puede indicar que no se encontraron datos.");
                    return NotFound($"No se pudo generar el recibo para el pago {IdPago}. Verifique si los datos existen o si hay un error en la generación.");
                }

                _logger.LogInformation($"Controlador: Recibo PDF generado y enviado para Pago Id: {IdPago}.");
                // Devuelve el PDF con un nombre de archivo amigable
                return File(pdfBytes, "application/pdf", $"Recibo_Municipal_Pago_{IdPago}.pdf");
            }
            catch (ApplicationException appEx)
            {
                // Captura las excepciones controladas lanzadas por el servicio
                _logger.LogError(appEx, $"Controlador: Error de aplicación al generar recibo: {appEx.Message}");
                return BadRequest($"Error en el proceso de generación de recibo: {appEx.Message}");
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción inesperada
                _logger.LogError(ex, $"Controlador: Error inesperado al generar recibo para Pago: {IdPago}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno del servidor al generar el recibo.");
            }
        }

    }

}
