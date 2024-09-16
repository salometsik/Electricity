using Electricity.Application.Contracts.Infrastructure;
using Electricity.Infrastructure.Dataset;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IRetrieveDataService, RetrieveDataService>();
            return services;
        }
    }
}
