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
    public class ContribuyentesImpuestosVariablesController : ControllerBase
    {
        private readonly ILogger<ContribuyentesImpuestosVariablesController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<ContribuyentesImpuestosVariables> _baseService;

        public ContribuyentesImpuestosVariablesController(IBaseService<ContribuyentesImpuestosVariables> baseService, ILogger<ContribuyentesImpuestosVariablesController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<ContribuyentesImpuestosVariables>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<ContribuyentesImpuestosVariables>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los impuestos variables");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<ContribuyentesImpuestosVariables>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<ContribuyentesImpuestosVariables>>.Exitoso(resultadoMapeado, "Listado de auditorias obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<ContribuyentesImpuestosVariables>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<ContribuyentesImpuestosVariables>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo impuestos variables con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<ContribuyentesImpuestosVariables>.Fallido($"No se encontró el impuesto con ID {id}"));

            var resultadoMapeado = _mapper.Map<ContribuyentesImpuestosVariables>(resultado);
            var resultadoDTO = ResultadoDTO<ContribuyentesImpuestosVariables>.Exitoso(resultadoMapeado, "Impuesto variable encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<ContribuyentesImpuestosVariables>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<ContribuyentesImpuestosVariables>>> Create([FromBody] ContribuyentesImpuestosVariablesDTO dto)
        {
            _logger.LogInformation("Creando una nueva auditoria");

            var entity = _mapper.Map<ContribuyentesImpuestosVariables>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<ContribuyentesImpuestosVariables>(createdEntity);

            var resultadoDTO = ResultadoDTO<ContribuyentesImpuestosVariables>.Exitoso(resultadoMapeado, "Impuesto creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] ContribuyentesImpuestosVariablesDTO dto)
        {
            _logger.LogInformation($"Actualizando auditoria con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el impuesto con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el impuesto con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Impuesto actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("Anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Anular(int id)
        {
            _logger.LogInformation($"Anulando Auditoria con ID {id}");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var existingEntity = await _baseService.GetByIdAsync(id);
            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el impuesto con ID {id} para anular"));

            existingEntity.Anulado = true;
            existingEntity.OpAnula = idUsuario;
            existingEntity.FecAnula = DateTime.Now;

            var annul = await _baseService.UpdateAsync(id, existingEntity);

            if (!annul)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el impuesto con ID {id} para anular"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Impuesto eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
