using Dapper;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Infrastructure.Persistence;

// Adapter que executa SQL explícito para gravar uma ordem aceita.
public sealed class DapperOrderRepository(IDbConnectionFactory connectionFactory) : IOrderRepository
{
    public async Task SaveAcceptedAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var assetCode = ToCode(order.Asset);
        var sideCode = order.Side == Side.Buy ? "C" : "V";
        var financialValue = decimal.Round(order.Price * order.Quantity, 2);
        var now = DateTime.UtcNow;

        // Garante que exista uma linha de exposição antes do UPDATE da transação.
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

        // Atualiza a exposição e seus campos de auditoria.
        const string updateExposureSql = """
            UPDATE exposicoes
            SET valor_atual = @CurrentValue,
                usuario_alteracao = @User,
                data_alteracao = @Now,
                versao = versao + 1
            WHERE ativo_codigo = @AssetCode;
            """;
        await connection.ExecuteAsync(new CommandDefinition(
            updateExposureSql,
            new { AssetCode = assetCode, CurrentValue = exposure.CurrentValue, User = user, Now = now },
            transaction,
            cancellationToken: cancellationToken));

        // Insere apenas ordens aceitas; ordens rejeitadas não chegam a este repositório.
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

        // Commit confirma ordem e exposição juntas; qualquer erro provoca rollback ao descartar a transação.
        await transaction.CommitAsync(cancellationToken);
    }

    // Converte o enum do domínio para o código textual persistido no banco.
    private static string ToCode(Asset asset) => asset switch
    {
        Asset.Petr4 => "PETR4",
        Asset.Vale3 => "VALE3",
        Asset.Viia4 => "VIIA4",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Ativo não suportado.")
    };
}
