using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlanPagoController : ControllerBase
    {
        private readonly ILogger<PlanPagoController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<PlanPago> _baseService;

        public PlanPagoController(IBaseService<PlanPago> baseService, ILogger<PlanPagoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<PlanPago>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<PlanPago>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los Planes de pago");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<PlanPago>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<PlanPago>>.Exitoso(resultadoMapeado, "Listado de Planes de pago obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<PlanPago>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<PlanPago>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo Planes de pago con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<PlanPago>.Fallido($"No se encontró el Plan de pago con ID {id}"));

            var resultadoMapeado = _mapper.Map<PlanPago>(resultado);
            var resultadoDTO = ResultadoDTO<PlanPago>.Exitoso(resultadoMapeado, "Plan de pago encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<PlanPago>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<PlanPago>>> Create([FromBody] PlanPagoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Plan de pago");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<PlanPago>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var entity = _mapper.Map<PlanPago>(dto);

            entity.OpCrea = idUsuario;
            entity.FecCrea = DateTime.Now;

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<PlanPago>(createdEntity);

            var resultadoDTO = ResultadoDTO<PlanPago>.Exitoso(resultadoMapeado, "Plan de pago creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PlanPagoDTO dto)
        {
            _logger.LogInformation($"Actualizando Plan de pago con ID {id}");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Plan de pago con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            existingEntity.OpMod = idUsuario;
            existingEntity.FecMod = DateTime.Now;

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el Plan de pago con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Plan de pago actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Anular(int id)
        {
            _logger.LogInformation($"Anulando Plan de pago con ID {id}");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var existingEntity = await _baseService.GetByIdAsync(id);
            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Plan de pago con ID {id} para anular"));

            existingEntity.Anulado = true;
            existingEntity.OpAnula = idUsuario;
            existingEntity.FecAnula = DateTime.Now;

            var updated = await _baseService.UpdateAsync(id, existingEntity);
            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo anular el Plan de pago con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Plan de pago anulado correctamente");
            return Ok(resultadoDTO);
        }

        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        //{
        //    _logger.LogInformation($"Eliminando Plan de pago con ID {id}");

        //    var deleted = await _baseService.DeleteAsync(id);

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Plan de pago con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Plan de pago eliminado correctamente");

        //    return Ok(resultadoDTO);
        //}

    }

}
