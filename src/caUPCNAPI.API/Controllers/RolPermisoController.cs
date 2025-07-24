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
        private readonly IRolService _rolService;

        public RolPermisoController(IBaseService<RolPermiso> baseService, ILogger<RolPermisoController> logger, IMapper mapper, IRolService rolService)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
            _rolService = rolService;
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

        [HttpDelete]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int idRol, int idPermiso)
        {
            _logger.LogInformation($"Eliminando conexion del permiso {idPermiso} con el rol {idRol}");

            var deleted = await _rolService.DeleteRolPermisoAsync(idRol, idPermiso);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la conexion del permiso {idPermiso} con el rol {idRol} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Permiso de rol eliminado correctamente");

            return Ok(resultadoDTO);
        }
    }

}
