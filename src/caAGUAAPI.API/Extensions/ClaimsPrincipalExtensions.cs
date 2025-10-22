using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Application.Services;
using caAGUAAPI.Infraestructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace caAGUAAPI.API.Extensions
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
