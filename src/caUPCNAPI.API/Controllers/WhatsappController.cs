using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly IWhatsappService _wppService;

        public WhatsappController(IWhatsappService wppService)
        {
            _wppService = wppService;
        }

        [HttpPost("EnvioMasivoPrueba")]
        public async Task<ActionResult<ResultadoDTO<object>>> Login(int idMunicipio)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultadoDTO<object>.Fallido("Datos inválidos."));

            var envio = await _wppService.EnvioMasivodePlantilla(idMunicipio);

            if (envio == false)
                return BadRequest(ResultadoDTO<object>.Fallido("Falló el envío masivo de mensajes"));

            return Ok(new ResultadoDTO<object>
            {
                EsExitoso = true,
                Mensaje = "Envío masivo exitoso",
                Datos = true
            });
        }
    }
}
