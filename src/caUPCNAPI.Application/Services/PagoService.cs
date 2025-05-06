using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Services
{
    public class PagoService : BaseService<Pago>, IPagoService
    {
        private readonly IPagoRepository _repository;
        
        public PagoService(IPagoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<List<Pago>> ObtenerPagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            return await _repository.PagosPorInmuebleAsync(idContribuyente, idInmueble, periodo);
        }

    }

}
