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
    public class ContribuyentesController : ControllerBase
    {
        private readonly ILogger<ContribuyentesController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<Contribuyente> _baseService;
        private readonly IContribuyenteService _contribuyenteService;
        private readonly IInmuebleService _inmuebleService;
        private readonly ITributoService _tributoService;
        private readonly IBaseService<ContribuyentesImpuestosVariables> _contribImpuesto;


        public ContribuyentesController(IBaseService<Contribuyente> baseService, IContribuyenteService contribuyenteService, IInmuebleService inmuebleService , 
                ITributoService tributoService, IBaseService<ContribuyentesImpuestosVariables> contribImpuesto, ILogger<ContribuyentesController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _contribuyenteService = contribuyenteService;
            _logger = logger;
            _mapper = mapper;
            _inmuebleService = inmuebleService;
            _tributoService = tributoService;
            _contribImpuesto = contribImpuesto;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Contribuyente>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los contribuyentes");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Contribuyente>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(resultadoMapeado, "Listado de contribuyentes obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("por-municipio")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Contribuyente>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> ContribuyentesporMunicipio()
        {
            _logger.LogInformation("Obteniendo todos los contribuyentes del municipio");

            // Obtener el IdMunicipio desde el token
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            // Obtener todos los contribuyentes y filtrar por municipio
            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1);

            var resultadoMapeado = _mapper.Map<IEnumerable<Contribuyente>>(filtrados);

            var resultadoDTO = ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(
                resultadoMapeado,
                "Listado de contribuyentes obtenido correctamente"
            );

            return Ok(resultadoDTO);
        }

        [HttpGet("celular/{numero}")]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> GetByCelular(string numero)
        {
            _logger.LogInformation("Buscando contribuyente por celular: {Numero}", numero);

            var contribuyente = await _baseService.FindAsync(c => c.Celular == numero);
            if (contribuyente == null)
                return NotFound(ResultadoDTO<Contribuyente>.Fallido("No se encontró un contribuyente con ese celular."));

            var dto = _mapper.Map<Contribuyente>(contribuyente);
            return Ok(ResultadoDTO<Contribuyente>.Exitoso(dto, "Contribuyente encontrado correctamente."));
        }

        [HttpGet("documento/{numero}")]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> GetByDocumento(string numero)
        {
            _logger.LogInformation("Buscando contribuyente por documento: {Numero}", numero);

            var contribuyente = await _baseService.FindAsync(c => c.NumeroDocumento == numero);
            if (contribuyente == null)
                return NotFound(ResultadoDTO<Contribuyente>.Fallido("No se encontró un contribuyente con ese número de documento."));

            var dto = _mapper.Map<Contribuyente>(contribuyente);
            return Ok(ResultadoDTO<Contribuyente>.Exitoso(dto, "Contribuyente encontrado correctamente."));
        }

        /// <summary>
        /// Obtiene una lista de los últimos 10 contribuyentes agregados.
        /// </summary>
        [HttpGet("ultimos20")]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> GetUltimos10Contribuyentes()
        {

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            // La excepción será manejada por un middleware o filtro de excepciones global
            var contribuyentes = await _contribuyenteService.GetLast10ContribuyentesAsync(idMunicipio);

            if (contribuyentes == null || !contribuyentes.Any())
            {
                _logger.LogInformation("Controlador: No se encontraron contribuyentes recientes.");
                return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(new List<Contribuyente>(), "No se encontraron contribuyentes recientes."));
            }

            _logger.LogInformation("Controlador: Devolviendo los últimos 10 contribuyentes.");
            return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(contribuyentes, $"Se encontraron {contribuyentes.Count()} contribuyentes recientes."));
        }

        [HttpGet("buscar-por-nombre/{nombre}")] // Nuevo endpoint para búsqueda
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultadoDTO<IEnumerable<Contribuyente>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultadoDTO<IEnumerable<Contribuyente>>))] // Para el error de longitud
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultadoDTO<IEnumerable<Contribuyente>>))]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> SearchContribuyentesByName(string nombre)
        {
            _logger.LogInformation($"Controlador: Recibida solicitud GET para /api/contribuyentes/buscar-por-nombre/{nombre}.");

            try
            {
                // Obtener el IdMunicipio desde el token
                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                var contribuyentes = await _contribuyenteService.SearchContribuyentesAsync(nombre, idMunicipio);

                if (contribuyentes == null || !contribuyentes.Any())
                {
                    _logger.LogInformation($"Controlador: No se encontraron contribuyentes para el término de búsqueda '{nombre}'.");
                    return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(new List<Contribuyente>(), $"No se encontraron contribuyentes para '{nombre}'."));
                }

                _logger.LogInformation($"Controlador: Devolviendo resultados para la búsqueda '{nombre}'.");
                return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(contribuyentes, $"Contribuyentes encontrados para '{nombre}'."));
            }
            catch (ApplicationException appEx)
            {
                // Captura la excepción lanzada por el servicio si el nombre es muy corto
                _logger.LogWarning(appEx, $"Controlador: Error de validación en búsqueda de contribuyentes: {appEx.Message}");
                return BadRequest(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido(appEx.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controlador: Error inesperado al buscar contribuyentes por nombre '{nombre}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("Ocurrió un error inesperado al procesar la búsqueda."));
            }
        }

        [HttpPost("actualizar-celular")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> ActualizarCelular([FromBody] ActualizarCelularDTO dto)
        {
            _logger.LogInformation($"Actualizando celular para el DNI {dto.NumeroDocumento}");

            var contribuyente = await _baseService.FindAsync(c => c.NumeroDocumento == dto.NumeroDocumento);

            if (contribuyente == null)
            {
                return NotFound(ResultadoDTO<string>.Fallido("Contribuyente no encontrado"));
            }

            contribuyente.Celular = dto.Celular;

            var actualizado = await _baseService.UpdateAsync(contribuyente.Id, contribuyente);

            if (!actualizado)
            {
                return BadRequest(ResultadoDTO<string>.Fallido("No se pudo actualizar el celular"));
            }

            return Ok(ResultadoDTO<string>.Exitoso("Celular actualizado correctamente"));
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo contribuyente con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Contribuyente>.Fallido($"No se encontró el contribuyente con ID {id}"));

            var resultadoMapeado = _mapper.Map<Contribuyente>(resultado);
            var resultadoDTO = ResultadoDTO<Contribuyente>.Exitoso(resultadoMapeado, "contribuyente encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Contribuyente>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Contribuyente>>> Create([FromBody] ContribuyenteDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Contribuyente");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var entity = _mapper.Map<Contribuyente>(dto);

            entity.IdMunicipio = idMunicipio;

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Contribuyente>(createdEntity);
            var listaImpuestos = await _tributoService.GetFijos(idMunicipio);

            if (dto.DomicilioInmueble)
            {
                var entityInmueble = new Inmueble
                {
                    IdContribuyente = resultadoMapeado.Id,
                    IdTipoInmueble = 1,
                    IdLocalidad = dto.IdLocalidad ?? 0,
                    Calle = dto.Calle ?? string.Empty,
                    Numero = dto.Numero ?? string.Empty,
                    Orientacion = dto.Orientacion ?? string.Empty,
                    Referencias = dto.Referencias ?? string.Empty,
                    EvaluoFiscal = 0,
                    EstadoId = 1,
                    AreaTotal = 0,
                    IdMunicipio = idMunicipio
                };

                var createdEntityInmueble = await _inmuebleService.AddAsync(entityInmueble);


                if (listaImpuestos.Any())
                {
                    foreach (var impuesto in listaImpuestos)
                    {
                        var relacion = new ContribuyentesImpuestosVariables
                        {
                            IdContribuyente = resultadoMapeado.Id,
                            IdTipoImpuesto = impuesto.Id,
                            IdInmueble = createdEntityInmueble.Id,
                            PeriodoDesde = DateTime.Now,
                            PeriodoHasta = DateTime.Now.AddYears(10)
                        };
                        await _contribImpuesto.AddAsync(relacion);
                    }
                }
            }

            var resultadoDTO = ResultadoDTO<Contribuyente>.Exitoso(resultadoMapeado, "Contribuyente creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ContribuyenteDTO dto)
        {
            _logger.LogInformation($"Actualizando contribuyente con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Contribuyente con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Contribuyente con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Contribuyente actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateContribuyenteEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del contribuyente {id} a 2 (Inactivo).");

            try
            {
                // Llama al método del servicio específico del contribuyente
                bool success = await _contribuyenteService.UpdateContribuyenteEstadoIdAsync(id);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"EstadoId del contribuyente {id} actualizado a (Inactivo) correctamente.");
                    return Ok(resultadoDTO);
                }
                else
                {
                    _logger.LogWarning($"No se pudo actualizar el EstadoId del contribuyente {id}. Es posible que no exista.");
                    return NotFound(ResultadoDTO<bool>.Fallido($"Contribuyente con Id: {id} no encontrado o no se pudo actualizar el estado."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el EstadoId del contribuyente {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<bool>.Fallido("Error interno del servidor al actualizar el estado del contribuyente."));
            }
        }

        [HttpGet("deudores3meses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultadoDTO<IEnumerable<Contribuyente>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultadoDTO<IEnumerable<Contribuyente>>))]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Contribuyente>>>> GetOverdueContribuyentes()
        {
            _logger.LogInformation("Controlador: Recibida solicitud GET para /api/contribuyentes/deudores.");

            try
            {
                var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
                if (idMunicipioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
                }

                int idMunicipio = int.Parse(idMunicipioClaim.Value);

                var contribuyentes = await _contribuyenteService.Contribuyentes3MesesAdeudados(idMunicipio);

                if (contribuyentes == null || !contribuyentes.Any())
                {
                    _logger.LogInformation("Controlador: No se encontraron contribuyentes con deudas pendientes de más de 3 meses.");
                    return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(new List<Contribuyente>(), "No se encontraron contribuyentes con deudas pendientes de más de 3 meses."));
                }

                _logger.LogInformation($"Controlador: Devolviendo {contribuyentes.Count()} contribuyentes deudores.");
                return Ok(ResultadoDTO<IEnumerable<Contribuyente>>.Exitoso(contribuyentes, $"Se encontraron {contribuyentes.Count()} contribuyentes con deudas pendientes de más de 3 meses."));
            }
            catch (ApplicationException appEx)
            {
                _logger.LogError(appEx, "Controlador: Error de aplicación al obtener contribuyentes deudores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<IEnumerable<Contribuyente>>.Fallido(appEx.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controlador: Error inesperado al obtener contribuyentes deudores.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("Ocurrió un error inesperado al procesar la solicitud."));
            }
        }


        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        //{
        //    _logger.LogInformation($"Eliminando Contribuyente con ID {id}");

        //    var deleted = await _baseService.DeleteAsync(id);

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Contribuyente con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Contribuyente eliminado correctamente");

        //    return Ok(resultadoDTO);
        //}

    }

}
