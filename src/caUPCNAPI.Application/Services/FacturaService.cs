using System;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
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
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.Data.SqlClient; // Asegúrate de que este DTO esté definido correctamente

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

        public async Task<byte[]> GenerarFacturaPorContribuyentePdf(int idContribuyente, string periodo, int idMunicipio)
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

                int facturaIdParaPdf = 0;
                string codigoBarraGeneradopdf = string.Empty;

                if (facturaExistente != null)
                {
                    _logger.LogInformation($"Ya existe una factura para el contribuyente Id: {idContribuyente} y período: {periodo}. No se generará una nueva.");
                    facturaIdParaPdf = facturaExistente.Id;
                    codigoBarraGeneradopdf = facturaExistente.codigobarra;

                }
                else
                {
                    // Calcular el monto total de la factura
                    decimal montoTotalFactura = inmueblesAgrupados.Sum(i => i.Monto);

                    // Crear la entidad Factura
                    var nuevaFactura = new Factura
                    {
                        IdMunicipio = idMunicipio,
                        IdContribuyente = idContribuyente,
                        Periodo = periodo,
                        FechaEmision = DateTime.Now,
                        FechaVencimiento = DateTime.Now,
                        MontoTotal = montoTotalFactura,
                        Estado = "Pendiente",
                        FechaCreacion = DateTime.Now,
                        codigobarra = ""
                    };

                    // Insertar la factura en la base de datos usando el repositorio
                    await _facturaRepo.AddAsync(nuevaFactura);

                    facturaIdParaPdf = nuevaFactura.Id;

                    // Llamar al SP para obtener el código de barras
                    var codigoBarraGenerado = await _facturaRepo.CodigoBarra(
                        idMunicipio,
                        nuevaFactura.Id,
                        nuevaFactura.FechaVencimiento,
                        nuevaFactura.MontoTotal,
                        nuevaFactura.codigobarra
                    );

                    
                    //Insertar update de codigo de barra
                    var factura = await _facturaRepo.GetByIdAsync(facturaIdParaPdf);
                    if (factura != null)
                    {
                        factura.codigobarra = codigoBarraGenerado;
                        await _facturaRepo.UpdateAsync(facturaIdParaPdf,factura);

                        codigoBarraGeneradopdf = factura.codigobarra;

                    }
                }

                // 3. Crear el documento PDF en memoria
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDocument = new PdfDocument(writer);
                    iText.Layout.Document document = new iText.Layout.Document(pdfDocument, iText.Kernel.Geom.PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);

                    // 4. Construir el HTML
                    string html = await ConstruirHtmlFacturaPorContribuyente(contribuyente, inmueblesAgrupados, periodo, codigoBarraGeneradopdf, idMunicipio);

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

        private async Task<string> ConstruirHtmlFacturaPorContribuyente(Contribuyente contribuyente, List<TributoAgrupadoDTO> inmueblesAgrupados, string periodo, string codigoBarraGeneradopdf, int idMunicipio)
        {
            string periodoFormateado = periodo; // Inicialmente, mantenemos el original
            if (periodo.Length == 6 && int.TryParse(periodo.Substring(0, 4), out int year) && int.TryParse(periodo.Substring(4, 2), out int month))
            {
                // Si el formato es YYYYMM y es un número válido, lo formateamos
                periodoFormateado = $"{month:D2}/{year}"; // Usamos :D2 para asegurar dos dígitos para el mes
            }

            //string codigoFactura = facturaId.ToString("D8");

            // Configurá el generador
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 50,
                    Width = 200,
                    Margin = 1
                }
            };

            // Generá el pixel data
            var pixelData = writer.Write(codigoBarraGeneradopdf);

            // Convertí a Bitmap
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            // Convertí a Base64
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            string base64CodigoBarras = Convert.ToBase64String(ms.ToArray());

            string html = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset=""UTF-8"">
                                <title>Factura de Tributos</title>
                                <style>
                                    /* Base Styles */
                                    body {{
                                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; /* Fuente más moderna */
                                        margin: 0;
                                        padding: 0;
                                        font-size: 13px; /* Ligeramente más grande */
                                        line-height: 1.6; /* Mayor interlineado para mejor legibilidad */
                                        color: #1a1a1a;
                                        background-color:  #ffffff; /* Un fondo muy sutil para el documento */
                                    }}
                                    .container {{
                                        padding: 30px; /* Mayor padding */
                                        max-width: 850px; /* Un poco más ancho */
                                        margin: 20px auto; /* Centrado y con margen superior/inferior */
                                        background-color: #ffffff;
                                        border-radius: 8px; /* Bordes ligeramente redondeados */
                                        border: 2px solid #000000;
                                    }}

                                    /* Header Section */
                                    .top-section {{
                                        display: flex;
                                        justify-content: space-between;
                                        align-items: center; /* Alineación centrada verticalmente para logo y texto */
                                        margin-bottom: 15px; /* Más espacio inferior */
                                        padding-bottom: 20px;
                                       
                                    }}
                                    .logo-container {{
                                        
                                        width: 180px; /* Logo un poco más grande */
                                        height: auto;
                                        flex-shrink: 0; /* Evita que el logo se encoja */
                                        padding-right: 30px; /* Más espacio */
                                    }}
                                    .logo-container img {{
                                        width: 100%;
                                        height: auto;
                                        display: block;
                                        border-radius: 4px; /* Pequeño redondeado para la imagen */
 
                                    }}
                                    .info-factura-contribuyente {{
                                        padding: 30px;
                                        flex-grow: 1;
                                        text-align: left;
                                        border: 2px solid #000000; border-radius: 8px;
                                    }}
                                    .info-factura-contribuyente p {{
                                        margin: 0;
                                        padding: 4px 0; /* Mayor padding vertical */
                                        font-size: 14px; /* Texto de info un poco más grande */
                                    }}
                                    .info-factura-contribuyente strong {{
                                        color: #1a1a1a;
                                    }}
                                    .info-factura-contribuyente .factura-numero {{
                                        font-size: 14px; /* Número de factura más grande */
                                        color: #1a1a1a;
                                        font-weight: bold;
                                    }}

                                    /* Section Titles */
                                    .titulo-inmueble {{
                                        font-size: 14px; /* Título del inmueble más grande */
                                        font-weight: 600; /* Ligeramente menos bold, más moderno */
                                        margin-top: 15px; /* Más espacio superior */
                                        margin-bottom: 6px; /* Más espacio inferior */
                                        color: #1a1a1a;
                                        padding-left: 10px;
                                        border: 2px solid #000000; 
                                        border-radius: 8px;  
                                        background-color: #e9ecef; /* Fondo más claro para cabeceras */
                                    }}
                                      /* Section Titles AZUL */
                                    .titulo-inmueble-azul {{
                                        font-size: 14px; /* Título del inmueble más grande */
                                        font-weight: 600; /* Ligeramente menos bold, más moderno */
                                        margin-top: 15px; /* Más espacio superior */
                                        margin-bottom: 6px; /* Más espacio inferior */
                                        color: #1a1a1a;
                                        padding-left: 10px;
                                        border: 2px solid ; 
                                        border-radius: 8px;  
                                        background-color: #1976D2; /* Fondo más claro para cabeceras */
                                        color: white; /* Letra blanca */
                                    }}

                                    /* Table Styles */
                                    .tabla-impuestos {{
                                        width: 100%;
                                        border-collapse: separate; /* Para permitir border-spacing */
                                        border-spacing: 0; /* Elimina espacio entre celdas */
                                        margin-bottom: 25px; /* Más espacio inferior */
                                        background-color: #fdfdfd;
                                        border-radius: 6px;
                                        overflow: hidden; /* Asegura que los bordes redondeados se apliquen bien */
                                    }}
                                    .tabla-impuestos th,
                                    .tabla-impuestos td {{
                                        padding: 6px 15px; /* Mayor padding en celdas */
                                        text-align: left;
                                        vertical-align: middle; /* Alineación vertical al medio */
                                        border-bottom: 1px solid #eeeeee; /* Solo borde inferior */
                                    }}
                                    .tabla-impuestos th {{
                                        background-color: #e9ecef; /* Fondo más claro para cabeceras */
                                        color: #495057;
                                        font-weight: 600;
                                        text-transform: uppercase; /* Texto en mayúsculas */
                                        font-size: 10px;
                                    }}
                                    .tabla-impuestos tr:last-child td {{
                                        border-bottom: none; /* Elimina el borde inferior de la última fila */
                                    }}
                                    .tabla-impuestos td.text-right,
                                    .tabla-impuestos th.text-right {{
                                        text-align: right;
                                    }}

                                    /* Total Rows */
                                    .total-inmueble-row td {{
                                        padding: 6px 15px; /* Mayor padding en celdas */
                                        font-size: 14px;
                                        background-color: #CCCCCC; /* Fondo sutil para el total del inmueble */
                                        border-top: 2px solid #000000; /* Borde superior distintivo */
                                        font-weight: bold;
                                        color: #000000;

                                    }}
                                    .total-inmueble-row td:first-child {{
                                        text-align: left; /* Asegura que el texto ""Total Inmueble"" esté a la izquierda */
                                        font-weight: normal; /* El texto ""Total Inmueble"" no tan bold */
                                    }}
                                    .total-inmueble-row td.text-right {{
                                        font-size: 16px; /* Monto del total inmueble más grande */
                                    }}

                                    /* General Total */
                                    .total-general {{
                                        font-weight: bold;
                                        text-align: right;
                                        font-size: 16px; /* Monto total general más grande */
                                        margin-top: 30px; /* Más espacio superior */
                                        padding-top: 15px;
                                        color: #000000;
                                        background-color: #CCCCCC;border-bottom: 1px solid #eeeeee; /* Solo borde inferior */
                                    }}

                                    /* Footer */
                                    .footer {{
                                        text-align: center;
                                        margin-top: 50px; /* Más espacio */
                                        font-size: 11px;
                                        color: #888;
                                        padding-top: 15px;
                                        border-top: 1px solid #eee;
                                    }}
                                    .footer p {{
                                        margin: 2px 0;
                                    }}
                                    .barcode-img {{
                                        display: block;
                                        margin-left: auto;
                                        margin-right: 0;
                                    }}
                                </style>
                            </head>
                            <body>
                             ";

            var nombremunicipio = await _facturaRepo.GetMunicipio(idMunicipio);
            var logoMunicipio = await _facturaRepo.GetLogoMunicipio(idMunicipio);
            html += $@" <div class=""container"">
                <div class=""top-section"">
                    <div class=""logo-container"">
                        <center>
                        <img src=""wwwroot/img/{logoMunicipio}"" alt=""Logo Municipalidad"">
                        <h3 class=""titulo-inmueble-azul"">Municipalidad de {nombremunicipio}</h3>
                        </center>
                    </div>
                    <div class=""info-factura-contribuyente"">
                        <img src=""data:image/png;base64,{base64CodigoBarras}"" alt=""Código de barras"" class=""barcode-img"" />
                        <p class=""factura-numero""><strong>Factura Nro.:</strong> {codigoBarraGeneradopdf}</p>
                        <p><strong>Contribuyente:</strong> {contribuyente.Nombres} {contribuyente.Apellidos}</p>
                        <p><strong>DNI:</strong> {contribuyente.NumeroDocumento}</p>
                        <p><strong>Período:</strong> {periodoFormateado}</p>
                    </div>
                </div>
                <h3 class=""titulo-inmueble"">Detalle de Factura</h3>
                ";

            decimal totalGeneral = 0;

            foreach (var inmuebleAgrupado in inmueblesAgrupados)
            {
                html += $@"<h3 class=""titulo-inmueble-azul"">Inmueble: {inmuebleAgrupado.Direccion}</h3>
                    <table class=""tabla-impuestos"">
                        <thead>
                            <tr>
                                <th>Descripción</th>
                                <th class=""text-right"">Monto</th>
                            </tr>
                        </thead>
                        <tbody>";

                var detallesTributosTask = _repository.ObtenerDetalleTributoPorInmuebleAsync(contribuyente.Id, inmuebleAgrupado.IdInmueble, periodo);
                var detallesTributos = await detallesTributosTask;
                decimal totalInmueble = 0;
                                    

                foreach (var detalle in detallesTributos)
                {
                    html += $@"<tr><td>{detalle.Descripcion}</td><td class=""text-right"">$ {detalle.Monto.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("es-AR"))}</td></tr>";
                    totalInmueble += detalle.Monto;
                }

                html += $@"</tbody>
                        <tfoot>
                            <tr class=""total-inmueble-row"">
                                <td>Total Inmueble</td>
                                <td class=""text-right"">$ {totalInmueble.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("es-AR"))}</td>
                            </tr>
                        </tfoot>
                    </table>";

                totalGeneral += totalInmueble;
            }

            html += $@"<p class=""total-general""><strong>Total General: $</strong> {totalGeneral.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("es-AR"))}</p>           

            <div class=""footer"">
                <p>Municipalidad de {nombremunicipio} - {DateTime.Now.Year}</p>
            </div>
            </div>
        </body>
        </html>";

            return html;
        }

        //GENERAR RECIBO DE PAGO

        public async Task<byte[]> GenerarYGuardarReciboPDFAsync(int idPago, int idContribuyente, int idMunicipio)
        {
            _logger.LogInformation($"Iniciando generación y guardado de recibo PDF para Pago: {idPago}, Contribuyente: {idContribuyente}, Municipio: {idMunicipio}.");

            var dataParaRecibo = await _facturaRepo.GetReciboDataForReceiptAsync(idPago, idContribuyente);

            if (dataParaRecibo == null)
            {
                _logger.LogWarning($"No se encontraron datos completos para generar el recibo del Pago: {idPago}, Contribuyente: {idContribuyente}, Municipio: {idMunicipio}.");
                throw new ApplicationException($"No se encontraron datos para generar el recibo del pago {idPago}. Verifique los IDs.");
            }

            var documentoContribuyente = await _facturaRepo.GetContribuyente(idContribuyente);

            string numeroRecibo;
            var existingRecibo = await _facturaRepo.GetExistingReciboAsync(idPago, documentoContribuyente); // Changed argument

            if (existingRecibo == null)
            {
                // El recibo no existe, lo creamos
                var nuevoReciboDb = new Recibo
                {
                    IdPago = dataParaRecibo.Pago.IdPago,
                    FechaEmision = DateTime.Now,
                    MontoTotal = dataParaRecibo.Pago.MontoPagado ?? 0M,
                    Periodo = dataParaRecibo.Tributo?.Periodo ?? dataParaRecibo.Pago.Periodo,
                    NombreContribuyente = $"{dataParaRecibo.Contribuyente?.Nombres} {dataParaRecibo.Contribuyente?.Apellidos}",
                    DocumentoContribuyente = dataParaRecibo.Contribuyente?.NumeroDocumento ?? "N/A",
                    DireccionInmueble = $"{dataParaRecibo.Inmueble?.Calle} {dataParaRecibo.Inmueble?.Numero}",
                    IdMunicipio = idMunicipio
                };

                await _facturaRepo.AddReciboAsync(nuevoReciboDb);
                numeroRecibo = nuevoReciboDb.Id.ToString(); // Usamos el ID recién generado
                _logger.LogInformation($"Nuevo registro de Recibo con Id: {numeroRecibo} creado en BD para el Pago {idPago}.");
            }
            else
            {
                // El recibo ya existe, usamos su ID y no lo insertamos de nuevo
                numeroRecibo = existingRecibo.Id.ToString();
                _logger.LogInformation($"Recibo con Id: {numeroRecibo} ya existe en BD para el Pago {idPago}. Se omite la creación de un nuevo registro.");
            }

            // Obtener el nombre del municipio
            var nombremunicipio = await _facturaRepo.GetMunicipio(idMunicipio);
            

            string htmlContent = BuildReciboHtml(
                dataParaRecibo.Pago,
                dataParaRecibo.Contribuyente,
                dataParaRecibo.Inmueble,
                dataParaRecibo.MedioPago,
                numeroRecibo,
                nombremunicipio // Pasamos el nombre del municipio al HTML
            );

            byte[] pdfBytes;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDocument = new PdfDocument(writer);
                    iText.Layout.Document document = new iText.Layout.Document(pdfDocument, PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);

                    using (MemoryStream htmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent)))
                    {
                        HtmlConverter.ConvertToPdf(htmlStream, pdfDocument);
                    }

                    document.Close();
                    pdfBytes = ms.ToArray();
                }
            }
            catch (Exception pdfEx)
            {
                _logger.LogError(pdfEx, $"Error al convertir HTML a PDF para el recibo del Pago {idPago}.");
                throw new ApplicationException("Error al generar el archivo PDF del recibo.", pdfEx);
            }

            _logger.LogInformation($"Generación de recibo PDF para Pago {idPago} finalizada con éxito.");
            return pdfBytes;
        }

        private string BuildReciboHtml(Pago pago, Contribuyente contribuyente, Inmueble inmueble, MedioPago medioPago, string numeroRecibo, string nombremunicipio)
        {
            string html = $@"
            <!DOCTYPE html>
                <html lang=""es"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Recibo de Pago Municipal</title>
                    <style>
                        body {{
                            font-family: 'Inter', sans-serif;
                            margin: 0;
                            padding: 0;
                            background-color: #f8fafc; /* Color de fondo claro */
                            color: #334155; /* Texto gris oscuro */
                        }}
                        .container {{
                            max-width: 700px;
                            margin: 30px auto;
                            background-color: #ffffff;
                            border-radius: 12px;
                            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
                            padding: 30px;
                            border: 1px solid #e2e8f0; /* Borde gris claro */
                        }}
                        .header {{
                            text-align: center;
                            margin-bottom: 25px;
                        }}
                        .header h1 {{
                            font-size: 28px;
                            font-weight: 700;
                            color: #1e293b;
                            margin-bottom: 10px;
                        }}
                        .header p {{
                            font-size: 14px;
                            color: #64748b; /* Gris medio */
                        }}
                        .section-title {{
                            font-size: 18px;
                            font-weight: 600;
                            color: #1e293b;
                            border-bottom: 1px solid #cbd5e1; /* Borde gris */
                            padding-bottom: 8px;
                            margin-bottom: 15px;
                        }}
                        .data-grid {{
                            display: grid;
                            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                            gap: 15px;
                            margin-bottom: 20px;
                        }}
                        .data-item strong {{
                            display: block;
                            font-size: 13px;
                            color: #475569; /* Gris oscuro */
                            margin-bottom: 4px;
                        }}
                        .data-item span {{
                            font-size: 14px;
                            color: #1e293b; /* Gris muy oscuro */
                        }}
                        .table {{
                            font-size: 14px;
                            width: 100%;
                            border-collapse: collapse;
                            margin-bottom: 20px;
                        }}
                        .table th, .table td {{
                            padding: 10px;
                            text-align: left;
                            border-bottom: 1px solid #e2e8f0; /* Borde de tabla */
                        }}
                        .table th {{
                            background-color: #f1f4f9; /* Fondo azul claro para encabezado */
                            font-weight: 400;
                            color: #1e293b; 
                        }}
                        .total-amount {{
                            text-align: right;
                            font-size: 24px;
                            font-weight: 700;
                            color: #1e293b; 
                            margin-top: 20px;
                            padding-top: 15px;
                            border-top: 1px solid #e2e8f0; /* Borde superior */
                        }}
                        .footer-text {{
                            font-size: 12px;
                            color: #64748b; /* Gris medio */
                            text-align: center;
                            margin-top: 30px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h1>Comprobante de Pago</h1>
                        </div>

                        <div class=""section-title"">Detalles del Recibo</div>
                        <div class=""data-grid"">
                            <div class=""data-item"">
                                <strong>Número de Recibo</strong>
                                <span>{numeroRecibo}</span>
                            </div>
                            <div class=""data-item"">
                            </div>
                            <div class=""data-item"">
                                <strong>Fecha de Emisión</strong>
                                <span>{DateTime.Now:dd/MM/yyyy HH:mm}</span>
                            </div>
                        </div>

                        <div class=""section-title"">Datos del Contribuyente</div>
                        <div class=""data-grid"">
                            <div class=""data-item"">
                                <strong>Nombre Completo:</strong>
                                <span>{contribuyente?.Nombres} {contribuyente?.Apellidos}</span>
                            </div>
                            <div class=""data-item"">
                                <strong>Documento:</strong>
                                <span>{contribuyente?.NumeroDocumento}</span>
                            </div>
                            <div class=""data-item"">
                                <strong>Dirección de Inmueble:</strong>
                                <span>{inmueble?.Calle} {inmueble?.Numero} {inmueble?.Orientacion}</span>
                            </div>
                        </div>

                        <div class=""section-title"">Detalles del Tributo Pagado</div>
                        <table class=""table"">
                            <thead>
                                <tr>
                                    <th>Período</th>
                                    <th>Monto Pagado</th>
                                    <th>Fecha de Pago</th>
                                    <th>Medio de Pago</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>{pago?.Periodo}</td>
                                    <td>{pago?.MontoPagado:C}</td>
                                    <td>{pago?.FechaPago:dd/MM/yyyy}</td>
                                    <td>{medioPago?.Nombre}</td>
                                </tr>
                            </tbody>
                        </table>

                        <div class=""total-amount"">
                            Total Pagado: {pago?.MontoPagado:C}
                        </div>

                        <div class=""footer-text"">
                            <p>Gracias por su pago a la Municipalidad de {nombremunicipio}.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return html;
        }
    }
}

