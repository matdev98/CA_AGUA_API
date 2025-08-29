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
    public class ContribuyenteService : BaseService<Contribuyente>, IContribuyenteService
    {
        private readonly IContribuyenteRepository _repository;

        public ContribuyenteService(IContribuyenteRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Contribuyente>> GetByMunicipioIdAsync(int idMunicipio)
        {
            return await _repository.GetByMunicipioIdAsync(idMunicipio);
        }

        public async Task<bool> UpdateContribuyenteEstadoIdAsync(int id, int idUsuario)
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

        public async Task<IEnumerable<Contribuyente>> GetLast10ContribuyentesAsync(int idMunicipio)
        {
            try
            {

                var contribuyentes = await _repository.GetLast10AddedContribuyentesAsync(idMunicipio);
                return contribuyentes;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener los contribuyentes recientes.", ex);
            }
        }

        public async Task<IEnumerable<Contribuyente>> SearchContribuyentesAsync(string nombreBusqueda, int idMunicipio)
        {
            
            if (string.IsNullOrWhiteSpace(nombreBusqueda) || nombreBusqueda.Length < 3)
            {
                throw new ApplicationException("El término de búsqueda debe tener al menos 3 caracteres.");
            }

            try
            {
                var contribuyentes = await _repository.SearchContribuyentesByNameAsync(nombreBusqueda,idMunicipio);

                if (contribuyentes == null || !contribuyentes.Any())
                {
                    return new List<Contribuyente>(); // Devolver lista vacía en lugar de null
                }
                else
                {
                    // Aplicar Trim a cada contribuyente en la lista
                    foreach (var c in contribuyentes)
                    {
                        TrimContribuyenteStrings(c);
                    }
                }

                return contribuyentes;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar contribuyentes por nombre: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Contribuyente>> Contribuyentes3MesesAdeudados(int idMunicipio)
        {
            try
            {

                var contribuyentes = await _repository.Contribuyentes3MesesAdeudados(idMunicipio);

                if (contribuyentes == null || !contribuyentes.Any())
                {
                    return new List<Contribuyente>();
                }
                else
                {
                    foreach (var c in contribuyentes)
                    {
                        TrimContribuyenteStrings(c);
                    }
                }
                return contribuyentes;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener los contribuyentes deudores recientes.", ex);
            }
        }

        private void TrimContribuyenteStrings(Contribuyente contribuyente)
        {
            // Puedes agregar todas las propiedades string de tu entidad Contribuyente aquí
            if (contribuyente.NumeroDocumento != null) contribuyente.NumeroDocumento = contribuyente.NumeroDocumento.Trim();
            if (contribuyente.CUIL != null) contribuyente.CUIL = contribuyente.CUIL.Trim();
            if (contribuyente.Nombres != null) contribuyente.Nombres = contribuyente.Nombres.Trim();
            if (contribuyente.Apellidos != null) contribuyente.Apellidos = contribuyente.Apellidos.Trim();
            if (contribuyente.Calle != null) contribuyente.Calle = contribuyente.Calle.Trim();
            if (contribuyente.Numero != null) contribuyente.Numero = contribuyente.Numero.Trim();
            if (contribuyente.Orientacion != null) contribuyente.Orientacion = contribuyente.Orientacion.Trim();
            if (contribuyente.Referencias != null) contribuyente.Referencias = contribuyente.Referencias.Trim();
            if (contribuyente.Telefono != null) contribuyente.Telefono = contribuyente.Telefono.Trim();
            if (contribuyente.Celular != null) contribuyente.Celular = contribuyente.Celular.Trim();
            if (contribuyente.Email != null) contribuyente.Email = contribuyente.Email.Trim();
        }

        public async Task<bool> UpdateAsync(int idContribuyente, ContribuyenteDTO? dto, int idUsuario)
        {
            try
            {
                var updated = await _repository.UpdateAsync(idContribuyente, dto, idUsuario);
                return updated;
            }
            catch
            {
                throw new ApplicationException("Error al actualizar los datos del contribuyente.");
            }
        }

    }

}
