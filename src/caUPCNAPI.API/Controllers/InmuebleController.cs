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

        public InmuebleController(IBaseService<Inmueble> baseService, IInmuebleService inmuebleService , ILogger<InmuebleController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _inmuebleService = inmuebleService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetAllInmuebles()
        {
            _logger.LogInformation("Obteniendo todos las auditorias");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var resultado = await _baseService.GetAllAsync();
            var filtrados = resultado.Where(c => c.IdMunicipio == idMunicipio && c.EstadoId == 1);

            var resultadoMapeado = _mapper.Map<IEnumerable<Inmueble>>(filtrados);
            var resultadoDTO = ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(resultadoMapeado, "Listado de auditorias obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Inmueble>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Inmueble>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo auditorias con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Inmueble>.Fallido($"No se encontró la auditoria con ID {id}"));

            var resultadoMapeado = _mapper.Map<Inmueble>(resultado);
            var resultadoDTO = ResultadoDTO<Inmueble>.Exitoso(resultadoMapeado, "Auditoria encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("por-contribuyente/{contribuyenteId}")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Inmueble>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Inmueble>>>> GetByContribuyenteId(int contribuyenteId)
        {
            _logger.LogInformation($"Obteniendo inmuebles del contribuyente con ID {contribuyenteId}");

            var resultado = await _inmuebleService.GetByContribuyenteIdAsync(contribuyenteId);

            if (!resultado.Any())
                return NotFound(ResultadoDTO<IEnumerable<Inmueble>>.Fallido("No se encontraron inmuebles para este contribuyente."));

            var resultadoDTO = ResultadoDTO<IEnumerable<Inmueble>>.Exitoso(resultado, "Inmuebles del contribuyente obtenidos correctamente");

            return Ok(resultadoDTO);
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
