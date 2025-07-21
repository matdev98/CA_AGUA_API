using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IFacturaRepository : IBaseRepository<Factura>
    {
        Task<Factura> GetByContribuyenteAndPeriodoAsync(int idContribuyente, string periodo);
        Task AddReciboAsync(Recibo recibo);
        Task<ReciboDataDTO?> GetReciboDataForReceiptAsync(int idPago, int idContribuyente);
        Task<string> GetMunicipio(int idMunicipio);
        Task<string> GetContribuyente(int idContribuyente);
        Task<Recibo> GetExistingReciboAsync(int idPago, string documentoContribuyente);
        Task<string> CodigoBarra(int idMunicipio, int idFactura, DateTime fechaVencimiento, decimal montototal, string codigoBarraGenerado);
        Task<string> GetLogoMunicipio(int idMunicipio);
    }

}
