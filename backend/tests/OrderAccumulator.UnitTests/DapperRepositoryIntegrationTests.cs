using Dapper;
using Npgsql;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;
using OrderAccumulator.Infrastructure.Persistence;

namespace OrderAccumulator.UnitTests;

// Testes de integração dos adapters Dapper contra o PostgreSQL disponibilizado pelo Docker.
public sealed class DapperRepositoryIntegrationTests
{
    private const string AssetCode = "PETR4";
    private readonly string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ExchangeDatabase")
        ?? "Host=localhost;Port=5432;Database=exchange;Username=exchange;Password=exchange";

    [Fact]
    public async Task ExposureRepository_ShouldCreateAndReadExposure()
    {
        await ResetAssetAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperExposureRepository(factory);

            var exposure = await repository.GetOrCreateAsync(Asset.Petr4, CancellationToken.None);

            Assert.Equal(Asset.Petr4, exposure.Asset);
            Assert.Equal(0m, exposure.CurrentValue);
        }
        finally
        {
            await ResetAssetAsync();
        }
    }

    [Fact]
    public async Task OrderRepository_ShouldPersistOrderAndExposureTogether()
    {
        await ResetAssetAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperOrderRepository(factory);
            var order = Order.Create(Asset.Petr4, Side.Buy, 100, 12.34m);
            var exposure = Exposure.Empty(Asset.Petr4);
            Assert.True(exposure.TryRegister(order, out _));

            var saved = await repository.SaveAcceptedAsync(order, exposure, "integration-test", CancellationToken.None);

            Assert.True(saved);
            await using var connection = new NpgsqlConnection(connectionString);
            var persistedOrder = await connection.QuerySingleAsync<(int Quantity, decimal Price)>(
                "SELECT quantidade, preco FROM ordens WHERE ativo_codigo = @AssetCode ORDER BY id DESC LIMIT 1",
                new { AssetCode });
            var persistedExposure = await connection.QuerySingleAsync<decimal>(
                "SELECT valor_atual FROM exposicoes WHERE ativo_codigo = @AssetCode",
                new { AssetCode });

            Assert.Equal(100, persistedOrder.Quantity);
            Assert.Equal(12.34m, persistedOrder.Price);
            Assert.Equal(1_234m, persistedExposure);
        }
        finally
        {
            await ResetAssetAsync();
        }
    }

    [Fact]
    public async Task OrderRepository_ShouldRejectStaleExposureSnapshot()
    {
        await ResetAssetAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperOrderRepository(factory);

            var firstOrder = Order.Create(Asset.Petr4, Side.Buy, 100, 10m);
            var firstExposure = Exposure.Empty(Asset.Petr4);
            Assert.True(firstExposure.TryRegister(firstOrder, out _));
            Assert.True(await repository.SaveAcceptedAsync(firstOrder, firstExposure, "integration-test", CancellationToken.None));

            // Este snapshot não conhece a primeira ordem e deve falhar pelo controle de concorrência.
            var staleOrder = Order.Create(Asset.Petr4, Side.Buy, 100, 10m);
            var staleExposure = Exposure.Empty(Asset.Petr4);
            Assert.True(staleExposure.TryRegister(staleOrder, out _));

            var saved = await repository.SaveAcceptedAsync(staleOrder, staleExposure, "integration-test", CancellationToken.None);

            Assert.False(saved);
            var orderCount = await CountOrdersAsync();
            Assert.Equal(1, orderCount);
        }
        finally
        {
            await ResetAssetAsync();
        }
    }

    // Limpa apenas os dados usados por este conjunto de testes.
    private async Task ResetAssetAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM ordens WHERE ativo_codigo = @AssetCode; UPDATE exposicoes SET valor_atual = 0, versao = 1, usuario_alteracao = 'test-cleanup', data_alteracao = NOW() WHERE ativo_codigo = @AssetCode;", new { AssetCode });
    }

    private async Task<int> CountOrdersAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ordens WHERE ativo_codigo = @AssetCode", new { AssetCode });
    }
}
