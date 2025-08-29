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

        public async Task<bool> UpdateInmuebleEstadoIdAsync(int id, int idUsuario)
        {
            try
            {
                bool success = await _repository.UpdateEstadoIdAsync(id, idUsuario);
                return success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Pago>> ObtenerPagosPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            return await _repository.PagosPorInmuebleAsync(idContribuyente, idInmueble, periodo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<PagoDetalleDTO>> GetPagosDetallePorFechasYMunicipioAsync(DateTime fechaInicio, DateTime fechaFin, int idMunicipio)
        {
            return await _repository.GetPagosDetallePorFechasYMunicipioAsync(fechaInicio, fechaFin, idMunicipio);
        }

        public async Task<CierreCaja> ProcesarCierreDeCajaAsync(int idUsuario, int idMunicipio)
        {
            try
            {

                var nuevoCierre = await _repository.RealizarCierreDeCajaAsync(idUsuario, idMunicipio);

                return nuevoCierre;
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException("Ocurrió un error inesperado al procesar el cierre de caja.", ex);
            }
        }

        public async Task<bool> AnularCierreCajaAsync(int idCierrre, int idUsuario)
        {
            try
            {
                var anularCierre = await _repository.AnularCierreCajaAsync(idCierrre, idUsuario);

                return anularCierre;
            }
            catch
            {
                throw new ApplicationException("Ocurrió un error al anular el cierre de caja.");
            }
        }

        public async Task<bool> Update(int id, Pago entidad)
        {
            try
            {
                var modificado = await _repository.Update(id, entidad);

                return modificado;
            }
            catch 
            { 
                throw new ApplicationException("Ocurrió un error al modificar el pago.");
            }
        }

        public async Task<Pago> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }

}
