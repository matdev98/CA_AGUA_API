using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static iText.IO.Util.IntHashtable;

namespace caMUNICIPIOSAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NoticiasController : ControllerBase
    {
        private readonly ILogger<NoticiasController> _logger;
        private readonly IMapper _mapper;
        private readonly IBaseService<Noticias> _baseService;

        public NoticiasController(ILogger<NoticiasController> logger, IMapper mapper, IBaseService<Noticias> baseService)
        {
            _logger = logger;
            _mapper = mapper;
            _baseService = baseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Noticias>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Noticias>>>> GetAllNoticias()
        {
            _logger.LogInformation("Obteniendo todas las noticias");

            var resultado = await _baseService.GetAllAsync();
            
            if (resultado.Count() == 0)
                return NotFound(ResultadoDTO<Noticias>.Fallido("No se encontraron noticias."));

            var resultadoMapeado = _mapper.Map<IEnumerable<Noticias>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Noticias>>.Exitoso(resultadoMapeado, "Listado de noticias obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [HttpGet("getPeriodo/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Noticias>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Noticias>>>> GetByPeriodo(string periodo)
        {
            _logger.LogInformation($"Obteniendo las noticias activas para el periodo {periodo}");

            var resultado = await _baseService.GetAllAsync();
            var periodoNum = int.Parse(periodo);
            var filtrados = resultado.Where(c =>
                int.Parse(c.FechaDesde) >= periodoNum &&
                int.Parse(c.FechaHasta) <= periodoNum &&
                c.Anulado == false);

            if (filtrados.Count() == 0)
                return NotFound(ResultadoDTO<Noticias>.Fallido($"No se encontraron noticias activas para el periodo {periodo}."));

            var resultadoMapeado = _mapper.Map<IEnumerable<Noticias>>(filtrados);

            var resultadoDTO = ResultadoDTO<IEnumerable<Noticias>>.Exitoso(
                resultadoMapeado,
                "Listado de noticias obtenido correctamente"
            );

            return Ok(resultadoDTO);
        }

        [HttpGet("getId/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Noticias>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Noticias>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo noticia con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Noticias>.Fallido($"No se encontró la noticia con ID {id}"));

            var resultadoMapeado = _mapper.Map<Noticias>(resultado);
            var resultadoDTO = ResultadoDTO<Noticias>.Exitoso(resultadoMapeado, "Noticia encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Noticias>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Noticias>>> Create([FromBody] NoticiaDTO dto)
        {
            _logger.LogInformation("Creando una nueva noticia");

            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }
            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var entidad = _mapper.Map<Noticias>(dto);
            entidad.IdMunicipio = idMunicipio;
            entidad.OpCrea = idUsuario;

            var createdEntity = await _baseService.AddAsync(entidad);
            var resultadoMapeado = _mapper.Map<Noticias>(createdEntity);

            var resultadoDTO = ResultadoDTO<Noticias>.Exitoso(resultadoMapeado, "Noticia creada exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, resultadoDTO);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResultadoDTO<Noticias>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Noticias>>> Update(int id, [FromBody] NoticiaDTO dto)
        {
            _logger.LogInformation($"Actualizando la noticia con ID {id}");

            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingEntity = await _baseService.GetByIdAsync(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró la noticia con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            existingEntity.OpModi = idUsuario;
            existingEntity.FecModi = DateTime.Now;

            var updatedEntity = await _baseService.UpdateAsync(id, existingEntity);
            if (!updatedEntity)
                return NotFound(ResultadoDTO<Noticias>.Fallido($"No se pudo actualizar la noticia con ID {id}"));

            var resultadoDTO = ResultadoDTO<Noticias>.Exitoso(existingEntity, "Noticia actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [HttpPut("anular/{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Noticias>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Noticias>>> Anular(int id)
        {
            _logger.LogInformation($"Anulando la noticia con ID {id}");

            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var entidad = await _baseService.GetByIdAsync(id);
            if (entidad == null)
                return NotFound(ResultadoDTO<Noticias>.Fallido($"No se encontró la noticia con ID {id} para anular"));

            entidad.Anulado = true;
            entidad.OpAnula = idUsuario;
            entidad.FecAnula = DateTime.Now;

            var updatedEntity = await _baseService.UpdateAsync(id, entidad);

            if (!updatedEntity)
                return NotFound(ResultadoDTO<Noticias>.Fallido($"No se pudo anular la noticia con ID {id}"));

            var resultadoDTO = ResultadoDTO<Noticias>.Exitoso(entidad, "Noticia anulada correctamente");

            return Ok(resultadoDTO);
        }

    }

}
