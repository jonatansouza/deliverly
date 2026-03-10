using DeliverlyCore.Infra.Persistence;
using DeliverlyCore.Infra.Persistence.Repositories;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.UseCases.TariffTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverlyCore.Infra
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<DeliverlyDbContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION")));

            services.AddScoped<ITariffRepository, TariffTableRepository>();

            services.AddScoped<CreateTariffTableUseCase>();
            services.AddScoped<GetTariffTableByIdUseCase>();
            services.AddScoped<ListTariffTablesUseCase>();
            services.AddScoped<UpdateTariffTableUseCase>();
            services.AddScoped<DeleteTariffTableUseCase>();

            return services;
        }
    }
}
