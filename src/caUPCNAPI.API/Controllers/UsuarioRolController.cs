using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuarioRolController : ControllerBase
    {
        private readonly ILogger<UsuarioRolController> _logger;
        private readonly IMapper _mapper;

        private readonly IBaseService<UsuarioRol> _baseService;
        private readonly IUserService _userService;
        private readonly IRolService _rolService;

        public UsuarioRolController(IBaseService<UsuarioRol> baseService, IUserService userService, ILogger<UsuarioRolController> logger, IMapper mapper, IRolService rolService)
        {
            _baseService = baseService;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _rolService = rolService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<UsuarioRol>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<UsuarioRol>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los Usuario Rol");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<UsuarioRol>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<UsuarioRol>>.Exitoso(resultadoMapeado, "Listado de Usuario Rol obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<UsuarioRol>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<RolDTO>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo Usuario Rol con ID {id}");

            var resultado = await _userService.GetRolByIdUsuario(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<RolDTO>.Fallido($"No se encontró el Usuario Rol con ID {id}"));

            var resultadoMapeado = new RolDTO
            {
                NombreRol = resultado.NombreRol,
                Descripcion = resultado.Descripcion
            };
            var resultadoDTO = ResultadoDTO<RolDTO>.Exitoso(resultadoMapeado, "Usuario Rol encontrado correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<UsuarioRol>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<UsuarioRol>>> Create([FromBody] UsuarioRolDTO dto)
        {
            _logger.LogInformation("Creando un nuevo Usuario Rol");

            var entity = _mapper.Map<UsuarioRol>(dto);
            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<UsuarioRol>(createdEntity);

            var resultadoDTO = ResultadoDTO<UsuarioRol>.Exitoso(resultadoMapeado, "Usuario Rol creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { Id = createdEntity.IdRol }, resultadoDTO);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Delete(int idUsuario, int idRol)
        {
            _logger.LogInformation($"Eliminando conexion del Usuario {idUsuario} con el Rol {idRol}");

            var deleted = await _rolService.DeleteUserRolAsync(idUsuario, idRol);

            if (!deleted)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el Usuario {idUsuario} con el Rol {idRol} para eliminar"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Usuario Rol eliminado correctamente");

            return Ok(resultadoDTO);
        }

    }

}
