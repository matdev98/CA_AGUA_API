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
            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var cantidad = await _datosService.CantidadContribuyentesActivosAsync(idMunicipio);

            var dto = new CantidadDTO { Cantidad = cantidad };

            var resultadoDTO = ResultadoDTO<CantidadDTO>.Exitoso(dto, "Cantidad de contribuyentes activos obtenida correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("CantidadInmuebles")]
        [ProducesResponseType(typeof(ResultadoDTO<CantidadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<CantidadDTO>>> TotalInmueblesRegistrados()
        {
            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var cantidad = await _datosService.TotalInmueblesRegistradosAsync(idMunicipio);

            var dto = new CantidadDTO { Cantidad = cantidad };

            var resultadoDTO = ResultadoDTO<CantidadDTO>.Exitoso(dto, "Total de inmuebles registrados obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("InmueblesPorTipo")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>>> TotalInmueblesPorTipo()
        {
            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _datosService.TotalInmueblesPorTipoAsync(idMunicipio);

            var resultadoDTO = ResultadoDTO<IEnumerable<InmueblesPorTipoDTO>>.Exitoso(
                resultado, "Listado de inmuebles agrupados por tipo obtenido correctamente"
            );

            return Ok(resultadoDTO);
        }

        [HttpGet("DeudaTotal")]
        [ProducesResponseType(typeof(ResultadoDTO<decimal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<decimal>>> ObtenerDeudaTotalAcumulada()
        {
            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var deudaTotal = await _datosService.DeudaTotalAcumuladaAsync(idMunicipio);

            var resultadoDTO = ResultadoDTO<decimal>.Exitoso(deudaTotal, "Deuda total acumulada obtenida correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("TopDeudores")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TopDeudoresDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TopDeudoresDTO>>>> TopContribuyentesConMasDeuda()
        {
            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _datosService.TopContribuyentesConMasDeudaAsync(idMunicipio);

            var resultadoDTO = ResultadoDTO<IEnumerable<TopDeudoresDTO>>.Exitoso(resultado, "Top 10 contribuyentes con más deuda");

            return Ok(resultadoDTO);
        }


    }

}
