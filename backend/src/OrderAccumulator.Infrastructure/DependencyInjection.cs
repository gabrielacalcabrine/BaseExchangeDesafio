using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;
using OrderAccumulator.Infrastructure.Persistence;

namespace OrderAccumulator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarPersistencia(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ExchangeDatabase")
            ?? throw new InvalidOperationException("ConnectionStrings:ExchangeDatabase não foi configurada.");

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddScoped<IExposureRepository, DapperExposureRepository>();
        services.AddScoped<IOrderRepository, DapperOrderRepository>();
        return services;
    }
}
