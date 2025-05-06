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
    public class DatosService : IDatosService
    {
        private readonly IDatosRepository _repository;
        
        public DatosService(IDatosRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CantidadContribuyentesActivosAsync()
        {
            return await _repository.GetCantidadContribuyentesActivosAsync();
        }

        public async Task<int> TotalInmueblesRegistradosAsync()
        {
            return await _repository.TotalInmueblesRegistradosAsync();
        }

        public async Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync()
        {
            return await _repository.TotalInmueblesPorTipoAsync();
        }

        public async Task<decimal> DeudaTotalAcumuladaAsync()
        {
            return await _repository.DeudaTotalAcumuladaAsync();
        }

        public async Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync()
        {
            return await _repository.TopContribuyentesConMasDeudaAsync();
        }


    }

}
