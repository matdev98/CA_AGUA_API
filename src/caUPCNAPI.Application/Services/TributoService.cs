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
    public class TributoService : BaseService<Tributo>, ITributoService
    {
        private readonly ITributoRepository _repository;
        
        public TributoService(ITributoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            try
            {
                var resultado = await _repository.GetByContribuyenteIdAsync(contribuyenteId);

                return ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>.Exitoso(
                    resultado,
                    "Tributos del contribuyente obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {

                return ResultadoDTO<IEnumerable<TributoContribuyenteDTO>>.Fallido(
                    $"Error al obtener tributos: {ex.Message}"
                );
            }
        }

        public async Task<List<TributoAgrupadoDTO>> ObtenerTributosAgrupadosAsync(int idContribuyente, string periodo)
        {
            return await _repository.ObtenerTributosAgrupados(idContribuyente, periodo);
        }

        public async Task<List<TributoContribuyenteDTO>> ObtenerDetalleTributoPorInmuebleAsync(int idContribuyente, int idInmueble, string periodo)
        {
            return await _repository.ObtenerDetalleTributoPorInmuebleAsync(idContribuyente, idInmueble, periodo);
        }


        public async Task GenerarTributosDelMesAsync(int IdMunicipio)
        {
            await _repository.GenerarTributosDelMesAsync(IdMunicipio);
        }

        public async Task<List<TributoContribuyenteDTO>> ObtenerTributosPorPeriodoAsync(string periodo, int idMunicipio)
        {
            return await _repository.ObtenerTributosPorPeriodoAsync(periodo, idMunicipio);
        }

    }

}
