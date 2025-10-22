using AutoMapper;
using caAGUAAPI.Application.DTOs;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.Services
{
    public class InicioCajaService : BaseService<InicioCaja>, IInicioCajaService
    {
        private readonly IInicioCajaRepository _repository;
        
        public InicioCajaService(IInicioCajaRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<InicioCaja> ProcesarInicioDeCajaAsync(int idUsuario, int idMunicipio, decimal monto)
        {
            try
            {

                var nuevoInicio = await _repository.RealizarInicioDeCajaAsync(idUsuario, idMunicipio, monto);

                return nuevoInicio;
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException("Ocurrió un error inesperado al procesar el cierre de caja.", ex);
            }
        }

        public async Task<IEnumerable<InicioCaja>> ObtenerInicioCajaPeriodoAsync(int idMunicipio, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                return await _repository.GetInicioCajaPeriodo(idMunicipio, fechaDesde, fechaHasta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener el cierre de caja por periodo.", ex);
            }
        }

        public async Task<bool> UpdateInicioCajaEstadoIdAsync(int id)
        {
            try
            {
                bool success = await _repository.UpdateEstadoIdAsync(id);
                return success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
