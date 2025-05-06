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
    public class TributoController : ControllerBase
    {
        private readonly ILogger<TributoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<Tributo> _baseService;
        private readonly ITributoService _tributoService;

        public TributoController(IBaseService<Tributo> baseService, ITributoService tributoService , ILogger<TributoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _tributoService = tributoService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Tributo>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Tributo>>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los tributos");

                var resultado = await _baseService.GetAllAsync();

                if (resultado == null)
                {
                    _logger.LogWarning("Resultado nulo al obtener tributos");
                    return StatusCode(500, ResultadoDTO<IEnumerable<Tributo>>.Fallido("Error interno: resultado nulo"));
                }

                var resultadoMapeado = _mapper.Map<IEnumerable<Tributo>>(resultado);

                var resultadoDTO = ResultadoDTO<IEnumerable<Tributo>>.Exitoso(resultadoMapeado, "Listado de tributos obtenido correctamente");

                return Ok(resultadoDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GetAll");
                return StatusCode(500, ResultadoDTO<IEnumerable<Tributo>>.Fallido("Error interno del servidor"));
            }

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Tributo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Tributo>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tributo con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Tributo>.Fallido($"No se encontró el tributo con ID {id}"));

            var resultadoMapeado = _mapper.Map<Tributo>(resultado);
            var resultadoDTO = ResultadoDTO<Tributo>.Exitoso(resultadoMapeado, "Tributo encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("por-contribuyente/{contribuyenteId}")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>>> GetByContribuyenteId(int contribuyenteId)
        {
            _logger.LogInformation($"Obteniendo tributos del contribuyente con ID {contribuyenteId}");

            var resultado = await _tributoService.GetByContribuyenteIdAsync(contribuyenteId);

            if (!resultado.EsExitoso || resultado.Datos == null || !resultado.Datos.Any())
                return NotFound(ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>.Fallido(
                    resultado.Errores ?? new List<string> { "No se encontraron tributos para este contribuyente." },
                    "Error al obtener tributos"
                ));

            return Ok(resultado);
        }

        [HttpGet("tributos/{idMunicipio}/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<List<TributoContribuyenteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<List<TributoContribuyenteDTO>>>> ObtenerTributosPorPeriodo(string periodo, int idMunicipio)
        {
            var tributos = await _tributoService.ObtenerTributosPorPeriodoAsync(periodo, idMunicipio);

            if (tributos == null || !tributos.Any())
            {
                return NotFound(new ResultadoDTO<List<TributoContribuyenteDTO>>
                {
                    EsExitoso = false,
                    Errores = new List<string> { "No se encontraron tributos para el periodo o municipalidad especificado." },
                    Mensaje = "Error al obtener los tributos."
                });
            }

            return Ok(new ResultadoDTO<List<TributoContribuyenteDTO>>
            {
                EsExitoso = true,
                Datos = tributos,
                Mensaje = "Tributos obtenidos correctamente."
            });
        }

        [HttpGet("tributoAgrupado/{idContribuyente:int}/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<List<TributoAgrupadoDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<List<TributoAgrupadoDTO>>>> ObtenerTributoAgrupado(int idContribuyente, string periodo)
        {
            var tributos = await _tributoService.ObtenerTributosAgrupadosAsync(idContribuyente, periodo);

            if (tributos == null || !tributos.Any())
            {
                return NotFound(new ResultadoDTO<List<TributoAgrupadoDTO>>
                {
                    EsExitoso = false,
                    Errores = new List<string> { "No se encontraron tributos para el contribuyente o periodo especificado." },
                    Mensaje = "Error al obtener los tributos agrupados."
                });
            }

            return Ok(new ResultadoDTO<List<TributoAgrupadoDTO>>
            {
                EsExitoso = true,
                Datos = tributos,
                Mensaje = "Tributos agrupados obtenidos correctamente."
            });
        }

        [HttpGet("detalleTributo/{idContribuyente:int}/{idInmueble:int}/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<List<TributoContribuyenteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<List<TributoContribuyenteDTO>>>> ObtenerDetalleTributoPorInmueble(int idContribuyente, int idInmueble, string periodo)
        {
            var detalles = await _tributoService.ObtenerDetalleTributoPorInmuebleAsync(idContribuyente, idInmueble, periodo);

            if (detalles == null || !detalles.Any())
            {
                return NotFound(new ResultadoDTO<List<TributoContribuyenteDTO>>
                {
                    EsExitoso = false,
                    Errores = new List<string> { "No se encontraron tributos con esos datos." },
                    Mensaje = "Error al obtener el detalle del tributo."
                });
            }

            return Ok(new ResultadoDTO<List<TributoContribuyenteDTO>>
            {
                EsExitoso = true,
                Datos = detalles,
                Mensaje = "Detalle del tributo obtenido correctamente."
            });
        }



        [HttpPost("generarDeuda")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<string>>> GenerarTributosDelMes(int IdMunicipio)
        {
            try
            {
                await _tributoService.GenerarTributosDelMesAsync(IdMunicipio);

                var resultadoDTO = ResultadoDTO<string>.Exitoso(
                          datos: "Tributos generados correctamente",
                          mensaje: "Tributos generados correctamente"
                );


                return Created(string.Empty, resultadoDTO);
            }
            catch (Exception ex)
            {
                var resultadoError = ResultadoDTO<string>.Fallido(
                    mensaje: "Error al generar tributos.",
                    errores: new List<string> { ex.Message }
                );

                return BadRequest(resultadoError);
            }
        }



        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] TributoDTO dto)
        {
            _logger.LogInformation($"Actualizando tributo con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tributo con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el tributo con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tributo actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando tributo con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el tributo con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Tributo eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
