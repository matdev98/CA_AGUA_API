using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IWhatsappService
    {
        Task<bool> EnvioMasivodePlantilla(int idMunicipio);
    }

}
