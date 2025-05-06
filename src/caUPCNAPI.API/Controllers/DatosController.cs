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
    public class DatosController : ControllerBase
    {
 
        private readonly IDatosService _datosService;

        public DatosController(IDatosService datosService, IMapper mapper)
        {
            _datosService = datosService;
        }

        [HttpGet("CantidadContribuyentes")]
        [ProducesResponseType(typeof(ResultadoDTO<CantidadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<CantidadDTO>>> CantidadContribuyentes()
        {
            var cantidad = await _datosService.CantidadContribuyentesActivosAsync();

            var dto = new CantidadDTO { Cantidad = cantidad };

            var resultadoDTO = ResultadoDTO<CantidadDTO>.Exitoso(dto, "Cantidad de contribuyentes activos obtenida correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("CantidadInmuebles")]
        [ProducesResponseType(typeof(ResultadoDTO<CantidadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<CantidadDTO>>> TotalInmueblesRegistrados()
        {
            var cantidad = await _datosService.TotalInmueblesRegistradosAsync();

            var dto = new CantidadDTO { Cantidad = cantidad };

            var resultadoDTO = ResultadoDTO<CantidadDTO>.Exitoso(dto, "Total de inmuebles registrados obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("InmueblesPorTipo")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>>> TotalInmueblesPorTipo()
        {
            var resultado = await _datosService.TotalInmueblesPorTipoAsync();

            var resultadoDTO = ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>.Exitoso(
                resultado, "Listado de inmuebles agrupados por tipo obtenido correctamente"
            );

            return Ok(resultadoDTO);
        }

        [HttpGet("DeudaTotal")]
        [ProducesResponseType(typeof(ResultadoDTO<decimal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<decimal>>> ObtenerDeudaTotalAcumulada()
        {
            var deudaTotal = await _datosService.DeudaTotalAcumuladaAsync();

            var resultadoDTO = ResultadoDTO<decimal>.Exitoso(deudaTotal, "Deuda total acumulada obtenida correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("TopDeudores")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TopDeudoresDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TopDeudoresDTO>>>> TopContribuyentesConMasDeuda()
        {
            var resultado = await _datosService.TopContribuyentesConMasDeudaAsync();

            var resultadoDTO = ResultadoDTO<IEnumerable<TopDeudoresDTO>>.Exitoso(resultado, "Top 10 contribuyentes con más deuda");

            return Ok(resultadoDTO);
        }


    }

}
