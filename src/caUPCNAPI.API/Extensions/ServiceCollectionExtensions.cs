using caMUNICIPIOSAPI.Application.Interfaces.Repositories;
using caMUNICIPIOSAPI.Application.Interfaces.Services;
using caMUNICIPIOSAPI.Application.Services;
using caMUNICIPIOSAPI.Infraestructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace caMUNICIPIOSAPI.API.Extensions
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
