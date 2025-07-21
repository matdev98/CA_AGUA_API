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
    public class InmuebleController : ControllerBase
    {
        private readonly ILogger<InmuebleController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Inmueble> _baseService;
        private readonly IInmuebleService _inmuebleService;
        private readonly ITributoService _tributoService;
        private readonly IBaseService<ContribuyentesImpuestosVariables> _contribImpuesto;

        public InmuebleController(IBaseService<Inmueble> baseService, IInmuebleService inmuebleService, ITributoService tributoService, IBaseService<ContribuyentesImpuestosVariables> contribImpuesto, ILogger<InmuebleController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _inmuebleService = inmuebleService;
            _logger = logger;
            _mapper = mapper;
            _tributoService = tributoService;
            _contribImpuesto = contribImpuesto;
        }

        [HttpGet("por-municipio")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetInmueblesByMunicipio()
        {
            _logger.LogInformation("Obteniendo inmuebles por municipio.");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            // La excepción será manejada por un middleware o filtro de excepciones global
            var inmuebles = await _inmuebleService.GetByMunicipioIdAsync(idMunicipio);

            if (inmuebles == null || !inmuebles.Any())
            {
                _logger.LogInformation("No se encontraron inmuebles.");
                return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(new List<Inmueble>(), "No se encontraron inmuebles."));
            }

            _logger.LogInformation("Devolviendo inmuebles por municipio.");
            return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(inmuebles, $"Se encontraron {inmuebles.Count()} inmuebles."));
        }

        [HttpGet("buscar-por-calle/{nombre}")] // Nuevo endpoint para búsqueda
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status400BadRequest)] // Para el error de longitud
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> SearchInmueblesByName(string nombre)
        {
            _logger.LogInformation($"Controlador: Recibida solicitud GET para /api/inmuebles/buscar-por-nombre/{nombre}.");

            try
            {
                var inmuebles = await _inmuebleService.SearchInmueblesAsync(nombre);

                if (inmuebles == null || !inmuebles.Any())
                {
                    _logger.LogInformation($"Controlador: No se encontraron inmuebles para el término de búsqueda '{nombre}'.");
                    return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(new List<Inmueble>(), $"No se encontraron inmuebles para '{nombre}'."));
                }

                _logger.LogInformation($"Controlador: Devolviendo resultados para la búsqueda '{nombre}'.");
                return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(inmuebles, $"Inmuebles encontrados para '{nombre}'."));
            }
            catch (ApplicationException appEx)
            {
                // Captura la excepción lanzada por el servicio si el nombre es muy corto
                _logger.LogWarning(appEx, $"Controlador: Error de validación en búsqueda de inmuebles: {appEx.Message}");
                return BadRequest(ResultadoDTO<IEnumerable<Inmueble>>.Fallido(appEx.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controlador: Error inesperado al buscar inmuebles por nombre '{nombre}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<IEnumerable<Inmueble>>.Fallido("Ocurrió un error inesperado al procesar la búsqueda."));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetAllInmuebles()
        {
            _logger.LogInformation("Obteniendo todos los inmuebles");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1);

            var resultadoMapeado = _mapper.Map<IEnumerable<Inmueble>>(filtrados);
            var resultadoDTO = ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(resultadoMapeado, "Listado de inmuebles obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Inmueble>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Inmueble>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo inmueble con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Inmueble>.Fallido($"No se encontró el inmueble con ID {id}"));

            var resultadoMapeado = _mapper.Map<Inmueble>(resultado);
            var resultadoDTO = ResultadoDTO<Inmueble>.Exitoso(resultadoMapeado, "Inmueble encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("por-contribuyente/{contribuyenteId}")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetByContribuyenteId(int contribuyenteId)
        {
            _logger.LogInformation($"Obteniendo inmuebles del contribuyente con ID {contribuyenteId}");

            var resultado = await _inmuebleService.GetByContribuyenteIdAsync(contribuyenteId);

            ResultadoDTO<IEnumerable<Inmueble>> resultadoDTO;

            if (!resultado.Any())
                resultadoDTO = ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(resultado, "El contribuyente no tiene inmuebles vinculados.");
            else
                resultadoDTO = ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(resultado, "Inmuebles del contribuyente obtenidos correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("ultimos20")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetUltimos10Inmuebles()
        {
            _logger.LogInformation("Controlador: Recibida solicitud GET para /api/inmuebles/ultimos10.");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            // La excepción será manejada por un middleware o filtro de excepciones global
            var inmuebles = await _inmuebleService.GetLastInmueblesAsync(idMunicipio);

            if (inmuebles == null || !inmuebles.Any())
            {
                _logger.LogInformation("Controlador: No se encontraron inmuebles recientes.");
                return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(new List<Inmueble>(), "No se encontraron inmuebles recientes."));
            }

            _logger.LogInformation("Controlador: Devolviendo los últimos 10 inmuebles.");
            return Ok(ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(inmuebles, $"Se encontraron {inmuebles.Count()} inmuebles recientes."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Inmueble>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Inmueble>>> CreateBase([FromBody] InmuebleDTO dto)
        {
            _logger.LogInformation("Creando una nueva auditoria");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var entity = _mapper.Map<Inmueble>(dto);

            entity.IdMunicipio = idMunicipio;

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Inmueble>(createdEntity);

            var resultadoDTO = ResultadoDTO<Inmueble>.Exitoso(resultadoMapeado, "Auditoria creada exitosamente");

            var listaImpuestos = await _tributoService.GetFijos(idMunicipio);

            if (listaImpuestos.Any())
            {
                foreach(var impuesto in listaImpuestos)
                {
                    var relacion = new ContribuyentesImpuestosVariables
                    {
                        IdContribuyente = createdEntity.IdContribuyente,
                        IdTipoImpuesto = impuesto.Id,
                        IdInmueble = createdEntity.Id,
                        PeriodoDesde = DateTime.Now,
                        PeriodoHasta = DateTime.Now.AddYears(10)
                    };
                    await _contribImpuesto.AddAsync(relacion);
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }



        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] InmuebleDTO dto)
        {
            _logger.LogInformation($"Actualizando auditoria con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la auditoria con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar la auditoria con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Auditoria actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")] // Ruta descriptiva
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateInmuebleEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del inmueble {id} a 2 (Inactivo).");

            try
            {

                bool success = await _inmuebleService.UpdateInmuebleEstadoIdAsync(id);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"EstadoId del inmueble {id} actualizado a (Inactivo) correctamente.");
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
        //    _logger.LogInformation($"Eliminando Auditoria con ID {id}");

        //    var deleted = await _baseService.DeleteAsync(id);

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la Auditoria con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Auditoria eliminada correctamente");

        //    return Ok(resultadoDTO);
        //}

    }

}
