using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories
{
    public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
    {
        private readonly AppDbContext _context;

        public FacturaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Factura> GetByContribuyenteAndPeriodoAsync(int idContribuyente, string periodo)
        {
            return await _context.Factura
                .FirstOrDefaultAsync(f => f.IdContribuyente == idContribuyente && f.Periodo == periodo);
        }

        public async Task AddReciboAsync(Recibo recibo)
        {
            _context.Recibo.Add(recibo);
            await _context.SaveChangesAsync();
        }

        public async Task<ReciboDataDTO> GetReciboDataForReceiptAsync(int idPago, int idContribuyente)
        {
            var data = await (from p in _context.Pagos
                              join c in _context.Contribuyentes on p.IdContribuyente equals c.Id
                              join i in _context.Inmuebles on p.Idinmueble equals i.Id
                              join mp in _context.MediosPago on p.IdMedioPago equals mp.Id
                              where p.IdPago == idPago && c.Id == idContribuyente
                              select new ReciboDataDTO
                              {
                                  Pago = p,
                                  Contribuyente = c,
                                  Inmueble = i,
                                  MedioPago = mp
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<string> GetMunicipio(int idMunicipio)
        {
            return await _context.Municipios
                .Where(m => m.Id == idMunicipio)
                .Select(m => m.Nombre)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        public async Task<Recibo> GetExistingReciboAsync(int idPago, string documentoContribuyente) // Changed parameter
        {
            return await _context.Recibo
                .FirstOrDefaultAsync(r => r.IdPago == idPago && r.DocumentoContribuyente == documentoContribuyente); // Changed condition
        }

        public async Task<string> GetContribuyente(int idContribuyente)
        {
            return await _context.Contribuyentes
                .Where(m => m.Id == idContribuyente)
                .Select(m => m.NumeroDocumento)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

    }


}
