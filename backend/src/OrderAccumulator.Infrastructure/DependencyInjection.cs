using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;
using OrderAccumulator.Infrastructure.Persistence;

namespace OrderAccumulator.Infrastructure;

// Composition da infraestrutura: registra Dapper, Npgsql e os adapters de persistência.
public static class DependencyInjection
{
    public static IServiceCollection AddExchangePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ExchangeDatabase")
            ?? throw new InvalidOperationException("ConnectionStrings:ExchangeDatabase não foi configurada.");

        // A fábrica recebe a connection string e o repositório depende apenas da sua abstração.
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddScoped<IExposureRepository, DapperExposureRepository>();
        services.AddScoped<IOrderRepository, DapperOrderRepository>();
        return services;
    }
}
