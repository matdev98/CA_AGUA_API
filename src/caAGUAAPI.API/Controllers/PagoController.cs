using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Application.Services;
using caAGUAAPI.Domain.Entities;
using caAGUAAPI.Infraestructure.Persistence;
using MercadoPago.Client.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caAGUAAPI.API.Controllers
{
  
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PagoController : ControllerBase
    {
        private readonly ILogger<PagoController> _logger;
        private readonly IMapper _mapper;

        //Esto no deberia estar aqui
        private readonly AppDbContext _context;

        private readonly IBaseService<Pago> _baseService;
        private readonly IPagoService _pagoService;
        private readonly ITributoService _tributoService;
        private readonly IContribuyenteService _contribuyenteService;

        public PagoController(AppDbContext dbContext, IBaseService<Pago> baseService, IPagoService pagoService, ITributoService tributoService, IContribuyenteService contribuyenteService, ILogger<PagoController> logger, IMapper mapper)
        {
            _context = dbContext;
            _baseService = baseService;
            _pagoService = pagoService;
            _tributoService = tributoService;
            _contribuyenteService = contribuyenteService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoDTO<IEnumerable<Pago>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<IEnumerable<Pago>>>> GetAllMunicipios()
        {
            _logger.LogInformation("Obteniendo todos los pagos");

            var resultado = await _baseService.GetAllAsync();
            var resultadoMapeado = _mapper.Map<IEnumerable<Pago>>(resultado);

            var resultadoDTO = ResultadoDTO<IEnumerable<Pago>>.Exitoso(resultadoMapeado, "Listado de pagos obtenido correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<Pago>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<Pago>>> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo pago con ID {id}");

            var resultado = await _baseService.GetByIdAsync(id);

            if (resultado == null)
                return NotFound(ResultadoDTO<Pago>.Fallido($"No se encontró el pago con ID {id}"));

            var resultadoMapeado = _mapper.Map<Pago>(resultado);
            var resultadoDTO = ResultadoDTO<Pago>.Exitoso(resultadoMapeado, "Pago encontrada correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpGet("por-inmueble/{idContribuyente}/{idInmueble}/{periodo}")]
        [ProducesResponseType(typeof(ResultadoDTO<List<Pago>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultadoDTO<List<Pago>>>> ObtenerPagosPorInmueble(int idContribuyente, int idInmueble, string periodo)
        {
            var pagos = await _pagoService.ObtenerPagosPorInmuebleAsync(idContribuyente, idInmueble, periodo);

            if (pagos == null || !pagos.Any())
            {
                return NotFound(new ResultadoDTO<List<Pago>>
                {
                    EsExitoso = false,
                    Errores = new List<string> { "No se encontraron pagos para los datos ingresados." },
                    Mensaje = "No hay pagos registrados."
                });
            }

            return Ok(new ResultadoDTO<List<Pago>>
            {
                EsExitoso = true,
                Datos = pagos,
                Mensaje = "Pagos obtenidos correctamente."
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDTO<Pago>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<Pago>>> Create([FromBody] PagoDTO dto)
        {
            _logger.LogInformation("Creando un nuevo pago");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Pago>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var entity = _mapper.Map<Pago>(dto);
             
            entity.FechaPago = DateTime.Now;
            entity.EstadoId = 1;
            entity.OpCrea = idUsuario; // Asignar el ID del usuario que crea el pago
            entity.FecCrea = DateTime.Now; // Asignar la fecha de creación

            var createdEntity = await _baseService.AddAsync(entity);
            var resultadoMapeado = _mapper.Map<Pago>(createdEntity);

            var resultadoDTO = ResultadoDTO<Pago>.Exitoso(resultadoMapeado, "Pago creado exitosamente");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.IdPago }, resultadoDTO);
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<string>>> Update(int id, [FromBody] PagoDTO dto)
        {
            _logger.LogInformation($"Actualizando pago con ID {id}");

            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idUsuarioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<string>>.Fallido("El Token no contiene IdUsuario"));
            }
            var idUsuario = int.Parse(idUsuarioClaim.Value);

            var existingEntity = await _pagoService.GetById(id);

            if (existingEntity == null)
                return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el pago con ID {id} para actualizar"));

            _mapper.Map(dto, existingEntity); // SOLO mapea campos no nulos

            existingEntity.OpMod = idUsuario; // Asignar el ID del usuario que modifica el pago
            existingEntity.FecMod = DateTime.Now; // Asignar la fecha de modificación

            //var updated = await _baseService.UpdateAsync(id, existingEntity);
            var updated = await _pagoService.Update(id, existingEntity);

            if (!updated)
                return NotFound(ResultadoDTO<string>.Fallido($"No se pudo actualizar el pago con ID {id}"));

            var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Pago actualizada correctamente");

            return Ok(resultadoDTO);
        }

        [Authorize]
        [HttpPut("anular/{id}")] // Ruta descriptiva
        [ProducesResponseType(typeof(ResultadoDTO<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoDTO<bool>>> UpdateInmuebleEstadoToInactive(int id)
        {
            _logger.LogInformation($"Intentando actualizar EstadoId del inmueble {id} a Inactivo.");

            try
            {
                var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (idUsuarioClaim == null)
                {
                    return Unauthorized(ResultadoDTO<IEnumerable<bool>>.Fallido("El Token no contiene IdUsuario"));
                }
                var idUsuario = int.Parse(idUsuarioClaim.Value);

                // Llama al método del servicio específico del inmueble
                bool success = await _pagoService.UpdateInmuebleEstadoIdAsync(id, idUsuario);

                if (success)
                {
                    var resultadoDTO = ResultadoDTO<bool>.Exitoso(true, $"EstadoId del inmueble {id} actualizado a Inactivo correctamente.");
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
        //    _logger.LogInformation($"Eliminando pago con ID {id}");

        //    // Esta llamada ahora debería ir al DeleteAsync de PagoService,
        //    // que a su vez usa PagoRepository para buscar por IdPago
        //    var deleted = await _pagoService.DeleteAsync(id); // Si _baseService es IPagoService, perfecto

        //    if (!deleted)
        //        return NotFound(ResultadoDTO<string>.Fallido($"No se encontró el pago con ID {id} para eliminar"));

        //    var resultadoDTO = ResultadoDTO<string>.Exitoso(null, "Pago eliminada correctamente");

        //    return Ok(resultadoDTO);
        //}

        [Authorize]
        [HttpGet("detalle-por-fechas")]
        [ProducesResponseType(typeof(ResultadoDTO<List<PagoDetalleDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResultadoDTO<List<PagoDetalleDTO>>>> GetPagosDetalle(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var idMunicipioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdMunicipio");
            if (idMunicipioClaim == null)
            {
                return Unauthorized(ResultadoDTO<IEnumerable<Contribuyente>>.Fallido("El Token no contiene IdMunicipio"));
            }

            int idMunicipio = int.Parse(idMunicipioClaim.Value);

            _logger.LogInformation($"Consultando detalle de pagos para Fechas: {fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}, Municipio: {idMunicipio}");

            if (fechaInicio > fechaFin)
            {
                return BadRequest(ResultadoDTO<string>.Fallido("La fecha de inicio no puede ser posterior a la fecha de fin."));
            }

            try
            {
                var pagosDetalle = await _pagoService.GetPagosDetallePorFechasYMunicipioAsync(fechaInicio, fechaFin, idMunicipio);

                if (pagosDetalle == null || !pagosDetalle.Any())
                {
                    return NotFound(ResultadoDTO<string>.Fallido("No se encontraron pagos con los criterios especificados."));
                }

                var resultadoDTO = ResultadoDTO<List<PagoDetalleDTO>>.Exitoso(pagosDetalle, "Detalle de pagos obtenido correctamente.");
                return Ok(resultadoDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de pagos por fechas y municipio.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResultadoDTO<string>.Fallido($"Error interno del servidor: {ex.Message}"));
            }
        }



        [Authorize]
        [HttpPost("crear-preferencia-mp")]
        [ProducesResponseType(typeof(ResultadoDTO<string>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResultadoDTO<string>>> CrearPreferenciaMP(string documento, string anioMes, string nombreProducto)
        {
            _logger.LogInformation("Creando preferencia de mercado pago");
            var preferencia = await _pagoService.CrearPreferenciaAsync(documento, anioMes, nombreProducto);

            if (preferencia == null)
            {
                return NotFound(ResultadoDTO<string>.Fallido("No se pudo crear la preferencia."));
            }

            var resultadoDTO = ResultadoDTO<string>.Exitoso(preferencia.InitPoint);

            return Ok(resultadoDTO);
        }


        [HttpPost("webhookmp")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Webhook2(
            [FromBody] MpWebhookDto? payload,
            [FromQuery] string? id,
            [FromQuery] string? topic,
            [FromQuery] string? type)
        {
            var eventType = payload?.Type ?? type ?? topic;
            var resourceId = payload != null ? payload.Data.Id.ToString() : id;

            if (string.IsNullOrWhiteSpace(resourceId))
                return Ok(); // MP reintenta si no respondés

            try
            {
                if (!string.Equals(eventType, "payment", StringComparison.OrdinalIgnoreCase))
                    return Ok(); // Ignoramos otros eventos

                var paymentClient = new PaymentClient();
                var payment = await paymentClient.GetAsync(long.Parse(resourceId));

                if (!string.Equals(payment.Status, "approved", StringComparison.OrdinalIgnoreCase))
                    return Ok(); // Solo procesamos pagos aprobados

                var paymentIdStr = payment.Id.ToString();

                var yaExiste = await _context.ComprobantesMercadoPago
                    .AnyAsync(c => c.ComprobanteMp == paymentIdStr);

                if (yaExiste)
                    return Ok(); // Ya registrado

                // Leer external_reference y extraer documento y anioMes
                var externalRef = payment.ExternalReference ?? "";
                var partes = externalRef.Split('|');
                var documento = partes.ElementAtOrDefault(0);
                var anioMes = partes.ElementAtOrDefault(1);


                if (string.IsNullOrWhiteSpace(documento) || string.IsNullOrWhiteSpace(anioMes))
                    return BadRequest("No se pudo obtener documento o anioMes desde external_reference.");

                var contribuyente = await _contribuyenteService.GetContribuyenteByDNI(documento);

                var deudas = await _tributoService.ObtenerTributosAgrupadosAsync(contribuyente.Id, anioMes);
                if (deudas == null || deudas.Count == 0)
                    return NotFound("No hay deudas para procesar.");

                await using var tx = await _context.Database.BeginTransactionAsync();

                foreach (var deuda in deudas)
                {
                    var pago = new Pago
                    {
                        IdContribuyente = contribuyente.Id,
                        FechaPago = DateTime.Now,
                        MontoPagado = deuda.Monto,
                        IdMedioPago = 7, // MercadoPago
                        Idinmueble = deuda.IdInmueble,
                        Periodo = deuda.Periodo,
                        EstadoId = 1,
                        IdMunicipio = deuda.IdMunicipio,
                        IdTributo = 0
                    };

                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync(); // para obtener IdPago

                    var comprobante = new ComprobantesMercadoPago
                    {
                        IdPago = (int)pago.IdPago,
                        ComprobanteMp = paymentIdStr,
                        ImportePago = deuda.Monto,
                        FechaCrea = DateTime.Now
                    };

                    _context.ComprobantesMercadoPago.Add(comprobante);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var token = await _pagoService.ObtenerTokenChattigoAsync();
                var telefono = contribuyente.Celular ?? ""; // Asegurate que esté en formato 549xxxxxxxxxx
                var nombre = $"{contribuyente.Nombres}";
                var mensaje = $"✅ ¡Excelente, {nombre}! Tu pago fue *registrado con éxito* 🎉. Gracias por confiar en el chatbot de la Municipalidad de Tintina 🏛️. ¡Estamos para ayudarte siempre! 🙌";

                await _pagoService.EnviarMensajeChattigoAsync(token, telefono, nombre, mensaje);

                return Ok("Webhook procesado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook de MercadoPago");
                return StatusCode(500, $"Error procesando webhook.{ex.Message}");
            }
        }

    }

}
