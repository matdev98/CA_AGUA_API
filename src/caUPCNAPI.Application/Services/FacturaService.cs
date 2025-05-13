using System;
using System.IO;
using System.Globalization;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Layout.Borders;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Geom;
using iText.Html2pdf;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities; // Asegúrate de que este DTO esté definido correctamente

namespace caMUNICIPIOSAPI.Application.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly ILogger<FacturaService> _logger;
        private readonly ITributoRepository _repository;
        private readonly IContribuyenteRepository _contribuyenteRepo;
        private readonly IFacturaRepository _facturaRepo;

        public FacturaService(ITributoRepository repository, IContribuyenteRepository contribuyenteRepo, IFacturaRepository facturaRepo , ILogger<FacturaService> logger)
        {
            _repository = repository;
            _contribuyenteRepo = contribuyenteRepo;
            _facturaRepo = facturaRepo;
            _logger = logger;
        }

        public async Task<byte[]> GenerarFacturaPorContribuyentePdf(int idContribuyente, string periodo)
        {
            try
            {
                var contribuyente = await _contribuyenteRepo.GetByIdAsync(idContribuyente);

                if (contribuyente == null)
                {
                    _logger.LogError($"No se encontró el contribuyente con Id: {idContribuyente}");
                    return null;
                }

                // 2. Obtener los inmuebles con tributos para el período
                var inmueblesAgrupados = _repository.ObtenerTributosAgrupados(idContribuyente, periodo).Result;

                if (inmueblesAgrupados == null || !inmueblesAgrupados.Any())
                {
                    _logger.LogWarning($"No se encontraron tributos para el contribuyente Id: {idContribuyente} en el período: {periodo}");
                    return null;
                }

                // Verificar si ya existe una factura para este contribuyente y período
                var facturaExistente = await _facturaRepo.GetByContribuyenteAndPeriodoAsync(idContribuyente, periodo);

                if (facturaExistente != null)
                {
                    _logger.LogInformation($"Ya existe una factura para el contribuyente Id: {idContribuyente} y período: {periodo}. No se generará una nueva.");

                }
                else
                {
                    // Calcular el monto total de la factura
                    decimal montoTotalFactura = inmueblesAgrupados.Sum(i => i.Monto);

                    // Crear la entidad Factura
                    var nuevaFactura = new Factura
                    {
                        IdContribuyente = idContribuyente,
                        Periodo = periodo,
                        FechaEmision = DateTime.Now,
                        FechaVencimiento = DateTime.Now,
                        MontoTotal = montoTotalFactura,
                        Estado = "Pendiente",
                        FechaCreacion = DateTime.Now
                    };

                    // Insertar la factura en la base de datos usando el repositorio
                    await _facturaRepo.AddAsync(nuevaFactura);
                }

                // 3. Crear el documento PDF en memoria
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDocument = new PdfDocument(writer);
                    iText.Layout.Document document = new iText.Layout.Document(pdfDocument, iText.Kernel.Geom.PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);

                    // 4. Construir el HTML
                    string html = await ConstruirHtmlFacturaPorContribuyente(contribuyente, inmueblesAgrupados, periodo);

                    // 5. Convertir el HTML a PDF
                    using (MemoryStream htmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                    {
                        HtmlConverter.ConvertToPdf(htmlStream, pdfDocument);
                    }

                    document.Close();
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar el PDF de la factura por contribuyente. ContribuyenteId: {idContribuyente}, Periodo: {periodo}", ex);
                throw;
            }
        }

        private async Task<string> ConstruirHtmlFacturaPorContribuyente(Contribuyente contribuyente, List<TributoAgrupadoDTO> inmueblesAgrupados, string periodo)
        {
            string html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"">
                    <title>Factura Contribuyente</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; }}
                        .container {{ padding: 20px; }}
                        .header {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }}
                        .logo {{ width: 100px; }}
                        .info-contribuyente {{ text-align: left; }}
                        .titulo-inmueble {{ font-size: 16px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; }}
                        .tabla-impuestos {{ width: 100%; border-collapse: collapse; margin-bottom: 10px; }}
                        .tabla-impuestos th, .tabla-impuestos td {{ border: 1px solid #ddd; padding: 6px; text-align: left; }}
                        .tabla-impuestos th {{ background-color: #f0f0f0; }}
                        .total-inmueble {{ font-weight: bold; text-align: right; margin-bottom: 15px; }}
                        .total-general {{ font-weight: bold; text-align: right; font-size: 18px; margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">
                                 <img src=""wwwroot/img/LogoTintina.jpg"" alt=""Logo Municipalidad"" style=""width: 100px;"">
                            </div>
                            <div class=""info-contribuyente"">
                                <p><strong>Contribuyente:</strong> {contribuyente.Nombres} {contribuyente.Apellidos}</p>
                                <p><strong>Documento:</strong> {contribuyente.NumeroDocumento}</p>
                                <p><strong>Periodo:</strong> {periodo}</p>
                            </div>
                        </div>";

            decimal totalGeneral = 0;

            foreach (var inmuebleAgrupado in inmueblesAgrupados)
            {
                html += $@"<h3 class=""titulo-inmueble"">Dirección: {inmuebleAgrupado.Direccion}</h3>
                           <table class=""tabla-impuestos"">
                               <thead>
                                   <tr>
                                       <th>Descripción</th>
                                       <th style=""text-align: right;"">Monto</th>
                                   </tr>
                               </thead>
                               <tbody>";

                var detallesTributosTask = _repository.ObtenerDetalleTributoPorInmuebleAsync(contribuyente.Id, inmuebleAgrupado.IdInmueble, periodo);
                Task.WaitAll(detallesTributosTask);
                var detallesTributos = detallesTributosTask.Result;
                decimal totalInmueble = 0;

                foreach (var detalle in detallesTributos)
                {
                    html += $@"<tr><td>{detalle.Descripcion}</td><td style=""text-align: right;"">{detalle.Monto.ToString("N2", CultureInfo.GetCultureInfo("es-AR"))}</td></tr>";
                    totalInmueble += detalle.Monto;
                }

                html += $@"</tbody>
                           <tfoot>
                               <tr>
                                   <td class=""total-inmueble"">Total Inmueble</td>
                                   <td style=""text-align: right; font-weight: bold;"">{totalInmueble.ToString("N2", CultureInfo.GetCultureInfo("es-AR"))}</td>
                               </tr>
                           </tfoot>
                       </table>";

                totalGeneral += totalInmueble;
            }

            html += $@"<p class=""total-general""><strong>Total General: $</strong> {totalGeneral.ToString("N2", CultureInfo.GetCultureInfo("es-AR"))}</p>
                    </div>
                </body>
                </html>";

            return html;
        }
    }
}

