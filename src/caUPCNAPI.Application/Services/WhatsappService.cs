using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Net.Http;
using System.Globalization;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class WhatsappService : IWhatsappService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IContribuyenteRepository _contribuyenteRepository;
        private readonly ITributoRepository _tributoRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WhatsappService(IFacturaRepository authRepository, IContribuyenteRepository contribuyenteRepository, ITributoRepository tributoRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _facturaRepository = authRepository;
            _contribuyenteRepository = contribuyenteRepository;
            _tributoRepository = tributoRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        //public async Task<bool> EnvioMasivodePlantilla(int idMunicipio)
        //{
        //    // 1. Obtener el municipio
        //    var municipio = await _facturaRepository.GetMunicipio(idMunicipio);
        //    if (municipio == null)
        //        return false;

        //    // 2. Calcular periodo actual (mes anterior)
        //    var fechaActual = DateTime.Now;
        //    var fechaPeriodo = fechaActual;
        //    var periodo = fechaPeriodo.ToString("yyyyMM");
        //    var periodoTexto = fechaPeriodo.ToString("MM/yyyy");

        //    // 3. Obtener contribuyentes con celular
        //    var contribuyentes = await _contribuyenteRepository.GetByMunicipioIdAsync(idMunicipio);
        //    var contribuyentesConCelular = contribuyentes
        //        .Where(c => !string.IsNullOrEmpty(c.Celular) && c.EstadoId == 1)
        //        .ToList();

        //    if (!contribuyentesConCelular.Any())
        //        return false;

        //    // 4. Obtener token de Chattigo
        //    var loginPayload = new ChattigoLoginRequest
        //    {
        //        username = "apimasive@munitintina",
        //        password = "api@2025"
        //    };

        //    var httpClient = _httpClientFactory.CreateClient();

        //    var loginResponse = await httpClient.PostAsJsonAsync("https://massive.chattigo.com/message/login", loginPayload);
        //    if (!loginResponse.IsSuccessStatusCode)
        //        return false;

        //    var loginData = await loginResponse.Content.ReadFromJsonAsync<ChattigoLoginResponse>();
        //    var token = loginData?.access_token;

        //    if (string.IsNullOrEmpty(token))
        //        return false;

        //    // 5. Armar destinos
        //    var destinos = new List<HsmDestination>();

        //    foreach (var c in contribuyentesConCelular)
        //    {
        //        var tributos = await _tributoRepository.ObtenerTributosAgrupados(c.Id, periodo);
        //        var total = tributos.Sum(t => t.Monto).ToString("N0");

        //        destinos.Add(new HsmDestination
        //        {
        //            id = c.Id.ToString(),
        //            destination = c.Celular,
        //            parameters = new List<string>
        //    {
        //        municipio,
        //        periodoTexto,
        //        total
        //    }
        //        });
        //    }

        //    // 6. Crear payload de HSM
        //    var hsmPayload = new HsmMessageRequest
        //    {
        //        did = "5493846412827",
        //        type = "HSM",
        //        channel = "WHATSAPP",
        //        hsm = new HsmContent
        //        {
        //            destinations = destinos,
        //            @namespace = "",
        //            template = "avisogeneraciondeuda",
        //            botAttention = true,
        //            languageCode = "es"
        //        }
        //    };

        //    // 7. Enviar mensaje
        //    var request = new HttpRequestMessage(HttpMethod.Post, "https://massive.chattigo.com/message/inbound");
        //    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //    request.Content = JsonContent.Create(hsmPayload);

        //    var response = await httpClient.SendAsync(request);
        //    return response.IsSuccessStatusCode;
        //}

        public async Task<bool> EnvioMasivodePlantilla(int idMunicipio)
        {
            // 1) Municipio
            var municipio = await _facturaRepository.GetMunicipio(idMunicipio);
            if (municipio == null) return false;

            // 2) Periodo actual (formato requerido)
            var fechaPeriodo = DateTime.Now;
            var periodo = fechaPeriodo.ToString("yyyyMM");
            var periodoTexto = fechaPeriodo.ToString("MM/yyyy");

            // 3) Contribuyentes activos con celular
            var contribuyentes = await _contribuyenteRepository.GetByMunicipioIdAsync(idMunicipio);
            var contribuyentesConCelular = contribuyentes
                .Where(c => c.EstadoId == 1 && !string.IsNullOrWhiteSpace(c.Celular))
                .ToList();

            if (!contribuyentesConCelular.Any()) return false;

            // 4) Token Chattigo
            var httpClient = _httpClientFactory.CreateClient();

            var loginPayload = new ChattigoLoginRequest
            {
                username = "apimasive@munitintina",
                password = "api@2025"
            };

            var loginResponse = await httpClient.PostAsJsonAsync("https://massive.chattigo.com/message/login", loginPayload);
            if (!loginResponse.IsSuccessStatusCode) return false;

            var loginData = await loginResponse.Content.ReadFromJsonAsync<ChattigoLoginResponse>();
            var token = loginData?.access_token;
            if (string.IsNullOrEmpty(token)) return false;

            // 5) Construir todos los destinos normalizando teléfonos y sumando importes
            var destinosTodos = new List<HsmDestination>();
            var formatoAR = CultureInfo.GetCultureInfo("es-AR");

            foreach (var c in contribuyentesConCelular)
            {
                var phone = NormalizePhone(c.Celular);
                if (string.IsNullOrEmpty(phone)) continue; // si quedó inválido, lo saltamos

                var tributos = await _tributoRepository.ObtenerTributosAgrupados(c.Id, periodo);
                var totalDecimal = tributos.Sum(t => t.Monto);
                var total = totalDecimal.ToString("N0", formatoAR);

                destinosTodos.Add(new HsmDestination
                {
                    id = c.Id.ToString(),
                    destination = phone,
                    parameters = new List<string>

            {
                municipio,   // nombre del municipio
                periodoTexto,       // "MM/yyyy"
                total               // importe formateado
            }
                });
            }

            if (!destinosTodos.Any()) return false;

            // 6) Enviar en lotes de hasta 1000
            const int MaxBatch = 1000;
            bool allOk = true;

            foreach (var lote in Chunk(destinosTodos, MaxBatch))
            {
                var hsmPayload = new HsmMessageRequest
                {
                    did = "5493846412827",
                    type = "HSM",
                    channel = "WHATSAPP",
                    hsm = new HsmContent
                    {
                        destinations = lote.ToList(),
                        @namespace = "",
                        template = "avisogeneraciondeudafactura",
                        botAttention = true,
                        languageCode = "es"
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://massive.chattigo.com/message/inbound");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(hsmPayload);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    allOk = false; // marcamos que hubo fallas, pero seguimos con los demás lotes
                }
            }

            return allOk;
        }

        /// <summary>
        /// Limpia caracteres no numéricos. Si tiene exactamente 10 dígitos, antepone "549".
        /// Si ya viene con 11/12/13 dígitos (ej. 549...), lo deja igual.
        /// </summary>
        private static string NormalizePhone(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var digits = new string(raw.Where(char.IsDigit).ToArray());

            // Regla pedida: si tiene 10 caracteres -> anteponer 549
            if (digits.Length == 10)
                return "549" + digits;

            // Si ya viene con prefijo (54..., 549...), lo dejamos como esté
            return digits;
        }

        /// <summary>
        /// Divide una secuencia en bloques del tamaño indicado.
        /// </summary>
        private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            var bucket = new List<T>(size);
            foreach (var item in source)
            {
                bucket.Add(item);
                if (bucket.Count == size)
                {
                    yield return bucket;
                    bucket = new List<T>(size);
                }
            }
            if (bucket.Count > 0)
                yield return bucket;
        }

    }
}
