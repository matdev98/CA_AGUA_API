using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolPermisoController : ControllerBase
    {
        private readonly ILogger<RolPermisoController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<RolPermiso> _baseService;

        public RolPermisoController(IBaseService<RolPermiso> baseService, ILogger<RolPermisoController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<RolPermiso>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<RolPermiso>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los permisos de roles");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<RolPermiso>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<RolPermiso>>.Exitoso(resultadoMapeado, "Listado de permisos de roles obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<RolPermiso>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<RolPermiso>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo permiso de rol con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<RolPermiso>.Fallido($"No se encontró el permiso de rol con ID {id}"));

            var resultadoMapeado = _mapper.Map<RolPermiso>(resultado);
            var resultadoDTO = ResultadoDTO<RolPermiso>.Exitoso(resultadoMapeado, "Permiso de rol encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<RolPermiso>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<RolPermiso>>> Create([FromBody] RolPermiso dto)
        {
            _logger.LogInformation("Creando un nuevo permiso de rol");

            var entity = _mapper.Map<RolPermiso>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<RolPermiso>(createdEntity);

            var resultadoDTO = ResultadoDTO<RolPermiso>.Exitoso(resultadoMapeado, "Permiso de rol creado exitosamente");

            return Created("", resultadoDTO);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] RolPermiso dto)
        {
            _logger.LogInformation($"Actualizando permiso de rol con ID {id}");

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el permiso de rol con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _baseService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el permiso de rol con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Permiso de rol actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando permiso de rol con ID {id}");

            var deleted = await _baseService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el permiso de rol con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Permiso de rol eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
