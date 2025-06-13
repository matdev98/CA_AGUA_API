using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class ValorTipoImpuestoService : BaseService<ValorTipoImpuesto>, IValorTipoImpuestoService
    {

        private readonly IValorTipoImpuestoRepository _valortipoImpuestoRepository; 
        public ValorTipoImpuestoService(
            IBaseRepository<ValorTipoImpuesto> baseRepository, 
            IValorTipoImpuestoRepository tipoImpuestoRepository,  
            IMapper mapper,                                  
            ILogger<ValorTipoImpuestoService> logger)   
            : base(baseRepository)
        {
            _valortipoImpuestoRepository = tipoImpuestoRepository;

        }

        public async Task<IEnumerable<NombreTipoImpuestoDTO>> GetNombreTipoImpuestoAsync()
        {
            try
            {
                return await _valortipoImpuestoRepository.GetNombreTipoImpuesto();
            }
            catch (Exception ex)
            {
                throw; // Propagate the exception
            }
        }
    }

}


