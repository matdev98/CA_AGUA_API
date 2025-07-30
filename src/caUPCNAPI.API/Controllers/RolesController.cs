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
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<Rol> _baseService;
        private readonly IRolService _rolService;

        public RolesController(IBaseService<Rol> baseService, IRolService rolService, ILogger<RolesController> logger, IMapper mapper)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
            _rolService = rolService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<RolConPermisoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<RolConPermisoDTO>>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los roles");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = new List<RolConPermisoDTO>();

            foreach(var x in resultado)
            {
                var componeMapeado = new RolConPermisoDTO
                {
                    IdRol = x.IdRol,
                    NombreRol = x.NombreRol,
                    Descripcion = x.Descripcion,
                    Permisos = await _rolService.GetPermisosRol(x.IdRol)
                };

                resultadoMapeado.Add(componeMapeado);
            }

            var resultadoDTO = ResultadoDTO<IEnumerable<RolConPermisoDTO>>.Exitoso(resultadoMapeado, "Listado de roles obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<RolConPermisoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<RolConPermisoDTO>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo rol con ID {id}");

            var resultado = await _rolService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<RolConPermisoDTO>.Fallido($"No se encontró el rol con ID {id}"));

            var resultadoMapeado = new RolConPermisoDTO
            {
                IdRol = id,
                NombreRol = resultado.NombreRol,
                Descripcion = resultado.Descripcion,
                Permisos = await _rolService.GetPermisosRol(id)
            };

            var resultadoDTO = ResultadoDTO<RolConPermisoDTO>.Exitoso(resultadoMapeado, "Rol encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Rol>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Rol>>> Create([FromBody] RolDTO dto)
        {
            _logger.LogInformation("Creando un nuevo rol");

            var entity = _mapper.Map<Rol>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Rol>(createdEntity);

            var resultadoDTO = ResultadoDTO<Rol>.Exitoso(resultadoMapeado, "Rol creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdRol }, resultadoDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] RolDTO dto)
        {
            _logger.LogInformation($"Actualizando rol con ID {id}");

            var existingEntity = await _rolService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el rol con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity);

            var updated = await _rolService.UpdateAsync(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el rol con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Rol actualizado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int id)
        {
            _logger.LogInformation($"Eliminando rol con ID {id}");

            var deleted = await _rolService.DeleteAsync(id);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el rol con ID {id} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Rol eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
