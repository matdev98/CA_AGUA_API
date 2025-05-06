using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Interfaces.Services
{
    public interface IInmuebleService : IBaseService<Inmueble>
    {
        Task<IEnumerable<Inmueble>> GetByContribuyenteIdAsync(int contribuyenteId);
    }

}
