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

        public async Task<int> CantidadContribuyentesActivosAsync(int idMunicipio)
        {
            return await _repository.GetCantidadContribuyentesActivosAsync(idMunicipio);
        }

        public async Task<int> TotalInmueblesRegistradosAsync(int idMunicipio)
        {
            return await _repository.TotalInmueblesRegistradosAsync(idMunicipio);
        }

        public async Task<List<InmueblesPorTipoDTO>> TotalInmueblesPorTipoAsync(int idMunicipio)
        {
            return await _repository.TotalInmueblesPorTipoAsync(idMunicipio);
        }

        public async Task<decimal> DeudaTotalAcumuladaAsync(int idMunicipio)
        {
            return await _repository.DeudaTotalAcumuladaAsync(idMunicipio);
        }

        public async Task<List<TopDeudoresDTO>> TopContribuyentesConMasDeudaAsync(int idMunicipio)
        {
            return await _repository.TopContribuyentesConMasDeudaAsync(idMunicipio);
        }


    }

}
