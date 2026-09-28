using Dapper;
using Npgsql;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;
using OrderAccumulator.Infrastructure.Persistence;

namespace OrderAccumulator.UnitTests;

public sealed class DapperRepositoryIntegrationTests
{
    private const string AssetCode = "PETR4";
    private readonly string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ExchangeDatabase")
        ?? "Host=localhost;Port=5432;Database=exchange;Username=exchange;Password=exchange";

    [Fact]
    public async Task RepositoryExposicao_DeveCriarELerExposicao()
    {
        await RestaurarAtivoAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperExposureRepository(factory);

            var exposure = await repository.ObterOuCriarAsync(Asset.Petr4, CancellationToken.None);

            Assert.Equal(Asset.Petr4, exposure.Asset);
            Assert.Equal(0m, exposure.CurrentValue);
        }
        finally
        {
            await RestaurarAtivoAsync();
        }
    }

    [Fact]
    public async Task RepositoryOrdem_DevePersistirOrdemEExposicaoJuntas()
    {
        await RestaurarAtivoAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperOrderRepository(factory);
            var order = Order.Criar(Asset.Petr4, Side.Buy, 100, 12.34m);
            var exposure = Exposure.Vazia(Asset.Petr4);
            Assert.True(exposure.TentarRegistrar(order, out _));

            var saved = await repository.SalvarOrdemAceitaAsync(order, exposure, "integration-test", CancellationToken.None);

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
            await RestaurarAtivoAsync();
        }
    }

    [Fact]
    public async Task RepositoryOrdem_DeverRejeitarSnapshotDeExposicaoDesatualizado()
    {
        await RestaurarAtivoAsync();
        try
        {
            var factory = new NpgsqlConnectionFactory(connectionString);
            var repository = new DapperOrderRepository(factory);

            var firstOrder = Order.Criar(Asset.Petr4, Side.Buy, 100, 10m);
            var firstExposure = Exposure.Vazia(Asset.Petr4);
            Assert.True(firstExposure.TentarRegistrar(firstOrder, out _));
            Assert.True(await repository.SalvarOrdemAceitaAsync(firstOrder, firstExposure, "integration-test", CancellationToken.None));

            var staleOrder = Order.Criar(Asset.Petr4, Side.Buy, 100, 10m);
            var staleExposure = Exposure.Vazia(Asset.Petr4);
            Assert.True(staleExposure.TentarRegistrar(staleOrder, out _));

            var saved = await repository.SalvarOrdemAceitaAsync(staleOrder, staleExposure, "integration-test", CancellationToken.None);

            Assert.False(saved);
            var orderCount = await ContarOrdensAsync();
            Assert.Equal(1, orderCount);
        }
        finally
        {
            await RestaurarAtivoAsync();
        }
    }

    private async Task RestaurarAtivoAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM ordens WHERE ativo_codigo = @AssetCode; UPDATE exposicoes SET valor_atual = 0, versao = 1, usuario_alteracao = 'test-cleanup', data_alteracao = NOW() WHERE ativo_codigo = @AssetCode;", new { AssetCode });
    }

    private async Task<int> ContarOrdensAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ordens WHERE ativo_codigo = @AssetCode", new { AssetCode });
    }
}
