using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Domain.Entities;

namespace caAGUAAPI.Application.Interfaces.Services
{
    public interface IWhatsappService
    {
        Task<bool> EnvioMasivodePlantilla(int idMunicipio);
    }

}
