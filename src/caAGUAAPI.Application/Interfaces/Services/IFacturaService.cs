using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;

namespace caAGUAAPI.Application.Interfaces.Services
{
    public interface IFacturaService
    {
        Task<byte[]> GenerarFacturaPorContribuyentePdf(int idContribuyente, string periodo, int idMunicipio, int idUsuario);
        Task<byte[]> GenerarYGuardarReciboPDFAsync(int idPago, int idContribuyente, int idMunicipio, int idUsuario);

    }

}
