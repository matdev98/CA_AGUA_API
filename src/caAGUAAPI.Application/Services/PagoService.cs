using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using MercadoPago.Config;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace caAGUAAPI.Application.Services
{
    public class PagoService : BaseService<Pago>, IPagoService
    {
        private readonly IPagoRepository _repository;
        private readonly ITributoRepository _tributoRepository;
        private readonly IBaseService<Contribuyente> _baseService;
        private readonly string _mpAccessToken;

        public PagoService(IPagoRepository repository, ITributoRepository tributoRepository, IBaseService<Contribuyente> baseService) : base(repository)
        {
            _repository = repository;
            _tributoRepository = tributoRepository;
            _baseService = baseService;

            DotNetEnv.Env.Load();

            _mpAccessToken = Environment.GetEnvironmentVariable("MP_ACCESS_TOKEN")
            ?? throw new InvalidOperationException("MP_ACCESS_TOKEN no configurado como variable de entorno.");
            
            MercadoPagoConfig.AccessToken = _mpAccessToken;

        }

        public async Task<bool> UpdateInmuebleEstadoIdAsync(int id, int idUsuario)
        {
            try
            {
                bool success = await _repository.UpdateEstadoIdAsync(id, idUsuario);
                return success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Pago>> ObtenerPagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            return await _repository.PagosPorInmuebleAsync(idContribuyente, idInmueble, periodo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio)
        {
            return await _repository.GetPagosDetallePorFechasYMunicipioAsync(fechaInicio, fechaFin, idMunicipio);
        }

        public async Task<CierreCaja> ProcesarCierreDeCajaAsync(int idUsuario, int idMunicipio)
        {
            try
            {

                var nuevoCierre = await _repository.RealizarCierreDeCajaAsync(idUsuario, idMunicipio);

                return nuevoCierre;
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException("Ocurrió un error inesperado al procesar el cierre de caja.", ex);
            }
        }

        public async Task<Preference> CrearPreferenciaAsync(string documento, string anioMes, string nombreProducto)
        {
            var contribuyente = await _baseService.FindAsync(c => c.NumeroDocumento == documento);
            if(contribuyente == null)
            {
                throw new Exception($"No se encontró el contribuyente con el documento {documento}");
            }
            // Calcular el importe total de la cuota (unit_price)
            var inmueblesAgrupados = _tributoRepository.ObtenerTributosAgrupados(contribuyente.Id, anioMes).Result;

            // Calcular el monto total de la factura
            decimal montoTotalFactura = inmueblesAgrupados.Sum(i => i.Monto);

            if (montoTotalFactura <= 0)
            {
                throw new Exception($"El importe total de la cuota es inválido.{montoTotalFactura}");
            }

            // Crear el cliente para la preferencia
            var client = new PreferenceClient();

            // Crear la preferencia
            var preferenceRequest = new PreferenceRequest
            {
                AutoReturn = "approved",
                //Expires = true,
                //ExpirationDateFrom = DateTime.Now,
                //ExpirationDateTo = DateTime.Now,
                NotificationUrl = "https://excelencia.myiphost.com:86/ApiMunicipios/api/v1/Pago/webhookmp",
                ExternalReference = $"{documento}|{anioMes}",
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "google.com.ar",
                    Pending = "",
                    Failure = ""
                },
                StatementDescriptor = "Municipalidad de Tintina",
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = nombreProducto,
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = montoTotalFactura,
                        CategoryId = "services"
                    }
                }
            };

            try
            {
                // Crear la preferencia en Mercado Pago
                var preference = await client.CreateAsync(preferenceRequest);
                return preference;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear la preferencia en Mercado Pago: {ex.Message}");
            }
        }

        public async Task<string> ObtenerTokenChattigoAsync()
        {
            var http = new HttpClient();
            var payload = new ChattigoLoginRequest
            {
                username = Environment.GetEnvironmentVariable("CHATTIGO_USERNAME")!,
                password = Environment.GetEnvironmentVariable("CHATTIGO_PASSWORD")!
            };

            var response = await http.PostAsJsonAsync("https://massive.chattigo.com/message/login", payload);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<ChattigoLoginResponse>();
            return data?.access_token!;
        }

        public async Task EnviarMensajeChattigoAsync(string token, string telefono, string nombre, string mensaje)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new ChattigoMessageRequest
            {
                Id = "1",
                Did = Environment.GetEnvironmentVariable("CHATTIGO_DID")!,
                Msisdn = telefono,
                Content = mensaje,
                Name = nombre
            };

            var response = await http.PostAsJsonAsync("https://massive.chattigo.com/message/inbound/message", payload);
            response.EnsureSuccessStatusCode();
        }
        public async Task<bool> AnularCierreCajaAsync(int idCierrre, int idUsuario)
        {
            try
            {
                var anularCierre = await _repository.AnularCierreCajaAsync(idCierrre, idUsuario);

                return anularCierre;
            }
            catch
            {
                throw new ApplicationException("Ocurrió un error al anular el cierre de caja.");
            }
        }

        public async Task<bool> Update(int id, Pago entidad)
        {
            try
            {
                var modificado = await _repository.Update(id, entidad);

                return modificado;
            }
            catch 
            { 
                throw new ApplicationException("Ocurrió un error al modificar el pago.");
            }
        }

        public async Task<Pago> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }

}
