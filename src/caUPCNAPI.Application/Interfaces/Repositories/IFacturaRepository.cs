using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Domain.Entities;

namespace caMUNICIPIOSAPI.Application.Interfaces.Repositories
{
    public interface IFacturaRepository : IBaseRepository<Factura>
    {
        Task<Factura> GetByContribuyenteAndPeriodoAsync(int idContribuyente, string periodo);
    }

}
