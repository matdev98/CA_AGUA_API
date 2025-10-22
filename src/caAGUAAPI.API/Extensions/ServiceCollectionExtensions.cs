using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Application.Interfaces.Services;
using caAGUAAPI.Application.Services;
using caAGUAAPI.Infraestructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace caAGUAAPI.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBaseServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            return services;
        }

        public static IServiceCollection AddBaseRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            return services;
        }
    }
}
