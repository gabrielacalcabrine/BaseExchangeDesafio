using Dapper;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Infrastructure.Persistence;

public sealed class DapperOrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperOrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> SalvarOrdemAceitaAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CriarConexaoAbertaAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var assetCode = ToCode(order.Asset);
        var sideCode = order.Side == Side.Buy ? "C" : "V";
        var financialValue = decimal.Round(order.Price * order.Quantity, 2);
        var now = DateTime.UtcNow;

        const string ensureExposureSql = """
            INSERT INTO exposicoes (ativo_codigo, valor_atual, usuario_inclusao, data_inclusao, versao)
            VALUES (@AssetCode, 0, @User, @Now, 1)
            ON CONFLICT (ativo_codigo) DO NOTHING;
            """;
        await connection.ExecuteAsync(new CommandDefinition(
            ensureExposureSql,
            new { AssetCode = assetCode, User = user, Now = now },
            transaction,
            cancellationToken: cancellationToken));

        const string updateExposureSql = """
            UPDATE exposicoes
            SET valor_atual = @CurrentValue,
                usuario_alteracao = @User,
                data_alteracao = @Now,
                versao = versao + 1
            WHERE ativo_codigo = @AssetCode
              AND valor_atual = @PreviousValue;
            """;
        var updatedRows = await connection.ExecuteAsync(new CommandDefinition(
            updateExposureSql,
            new
            {
                AssetCode = assetCode,
                CurrentValue = exposure.CurrentValue,
                PreviousValue = exposure.CurrentValue - order.SignedFinancialValue(),
                User = user,
                Now = now
            },
            transaction,
            cancellationToken: cancellationToken));

        if (updatedRows == 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        const string insertOrderSql = """
            INSERT INTO ordens (
                ativo_codigo,
                lado,
                quantidade,
                preco,
                valor_financeiro,
                usuario_inclusao,
                data_inclusao)
            VALUES (
                @AssetCode,
                @SideCode,
                @Quantity,
                @Price,
                @FinancialValue,
                @User,
                @Now);
            """;
        await connection.ExecuteAsync(new CommandDefinition(
            insertOrderSql,
            new
            {
                AssetCode = assetCode,
                SideCode = sideCode,
                Quantity = order.Quantity,
                Price = order.Price,
                FinancialValue = financialValue,
                User = user,
                Now = now
            },
            transaction,
            cancellationToken: cancellationToken));

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    private static string ToCode(Asset asset) => asset switch
    {
        Asset.Petr4 => "PETR4",
        Asset.Vale3 => "VALE3",
        Asset.Viia4 => "VIIA4",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Ativo não suportado.")
    };
}
