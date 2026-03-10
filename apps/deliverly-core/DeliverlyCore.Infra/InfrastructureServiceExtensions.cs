using DeliverlyCore.Infra.Persistence;
using DeliverlyCore.Infra.Persistence.Repositories;
using DeliverlyCore.Pricing.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverlyCore.Infra
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DeliverlyDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITariffRepository, TariffTableRepository>();

            return services;
        }
    }
}
