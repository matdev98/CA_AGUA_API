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
    //[Authorize]
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
        public async Task<IActionResult> GenerarFacturaPdf(int idContribuyente, string periodo)
        {
            try
            {
                _logger.LogInformation($"Generando PDF de factura para idContribuyente: {idContribuyente}, Periodo: {periodo}");
                byte[] pdfBytes = await _facturaService.GenerarFacturaPorContribuyentePdf(idContribuyente, periodo);

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

    }

}
