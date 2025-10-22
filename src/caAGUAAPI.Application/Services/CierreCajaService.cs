using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;
using ZXing;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace caAGUAAPI.Application.Services
{
    public class CierreCajaService : BaseService<CierreCaja>, ICierreCajaService
    {
        private readonly ICierreCajaRepository _repository;
        private readonly ILogger<CierreCajaService> _logger;
        private readonly IBaseService<CierreCaja> _baseService;

        public CierreCajaService(ICierreCajaRepository repository) : base(repository)
        {
            _repository = repository;
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

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosCerradosAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                return await _repository.PagosCerrados(idMunicipio, fechaDesde, fechaHasta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener los pagos cerrados.", ex);
            }
        }

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosSinCerrarAsync(int idMunicipio)
        {
            try
            {
                return await _repository.PagosSinCerrar(idMunicipio);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener los pagos sin cerrar.", ex);
            }
        }

        public async Task<IEnumerable<PagoCerradoDetalleDTO>> ObtenerPagosDeUnCierreAsync(int idMunicipio, int idCierre)
        {
            try
            {
                return await _repository.PagosdeunCierre(idMunicipio, idCierre);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener los pagos de un cierre específico.", ex);
            }
        }


        public async Task<IEnumerable<CierreCaja>> ObtenerCierreCajaPeriodoAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                return await _repository.GetCierreCajaPeriodo(idMunicipio, fechaDesde, fechaHasta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener el cierre de caja por periodo.", ex);
            }
        }

        public async Task<bool> UpdateCierreCajaEstadoIdAsync(int id)
        {
            try
            {
                bool success = await _repository.UpdateEstadoIdAsync(id);
                return success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> GenerarCierreCajaPdf(int idMunicipio, int idCierre, IEnumerable<PagoCerradoDetalleDTO> resultadoDTO)
        {
            try
            {
                var cierre = await _repository.GetByIdAsync(idCierre);

                if (cierre == null)
                {
                    _logger.LogError($"No se encontró el cierre con Id: {idCierre}");
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDocument = new PdfDocument(writer);
                    iText.Layout.Document document = new iText.Layout.Document(pdfDocument, iText.Kernel.Geom.PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);

                    string html = await ConstruirHtmlCierreCajaPdf(idMunicipio, idCierre, resultadoDTO);

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
                _logger.LogError($"Error al generar el PDF del cierre de caja. idMunicipio: {idMunicipio}, idCierre: {idCierre}", ex);
                throw;
            }
        }

        private async Task<string> ConstruirHtmlCierreCajaPdf(int idMunicipio, int idCierre, IEnumerable<PagoCerradoDetalleDTO> resultadoDTO)
        {
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
                                    .tabla-impuestos tr {{
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

            var nombremunicipio = await _repository.GetMunicipio(idMunicipio);
            var fechaCierre = await _repository.GetFechaCierre(idCierre);
            var logoMunicipio = await _repository.GetLogoMunicipio(idMunicipio);
            html += $@" <div class=""container"">
                                    <div class=""top-section"">
                                        <div class=""logo-container"">
                                           <center>
                                                <img src=""wwwroot/img/{logoMunicipio}"" alt=""Logo Municipalidad"">
                                            </center>
                                        </div>
                                        <div class=""info-factura-contribuyente"">
                                            <h2><center><strong>Informe de Cierre de Caja</strong></center></h3>
                                            <p><strong>Municipalidad de:</strong> {nombremunicipio}</p>
                                            <p><strong>Cierre de Caja:</strong> {idCierre}</p>
                                            <p><strong>Fecha de Cierre:</strong> {fechaCierre}</p>
                                        </div>
                                    </div>
                                  ";

            decimal totalGeneral = 0;

            html += $@"<h3 class=""titulo-inmueble-azul"">Detalle de Pagos</h3>
                                    <table class=""tabla-impuestos"">
                                        <thead>
                                            <tr>
                                                <th>Fecha</th>
                                                <th>Nombre del Contribuyente</th>
                                                <th>Direccion</th>
                                                <th>Medio de Pago</th>
                                                <th>Importe</th>
                                            </tr>
                                        </thead>
                                        <tbody>";

            foreach (var detalle in resultadoDTO)
            {
                html += $@"<tr>
                            <td>{detalle.FechaPago.Value.ToString("dd/MM/yyyy")}</td>
                            <td>{detalle.ApellidosContribuyente}, {detalle.NombresContribuyente}</td>
                            <td>{detalle.DireccionInmueble}</td>
                            <td>{detalle.NombreMedioPago}</td>
                            <td>$ {detalle.MontoPagado}</td>
                        </tr>";
                totalGeneral += detalle.MontoPagado ?? 0;
            }

            html += $@"</tbody></table>";

            html += $@"<p class=""total-general""><strong>Total General: $</strong> {totalGeneral.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("es-AR"))}</p>           

                                <div class=""footer"">
                                    <p>Municipalidad de {nombremunicipio} - {DateTime.Now.Year}</p>
                                </div>
                                </div>
                            </body>
                            </html>";

            return html;
        }

    }

}
