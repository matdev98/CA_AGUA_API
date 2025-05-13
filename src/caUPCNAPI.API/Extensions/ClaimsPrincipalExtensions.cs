using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace caMUNICIPIOSAPI.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetMunicipioId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == "IdMunicipio")?.Value;
            return int.TryParse(claim, out int idMunicipio) ? idMunicipio : null;
        }
    }
}
