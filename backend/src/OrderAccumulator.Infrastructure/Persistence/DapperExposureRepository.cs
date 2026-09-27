using Dapper;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Infrastructure.Persistence;

// Adapter de persistência: SQL explícito traduz a porta do domínio para PostgreSQL.
public sealed class DapperExposureRepository(IDbConnectionFactory connectionFactory) : IExposureRepository
{
    public async Task<Exposure> GetOrCreateAsync(Asset asset, CancellationToken cancellationToken)
    {
        var code = ToCode(asset);
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        // INSERT ... ON CONFLICT torna a criação idempotente quando duas requisições chegam juntas.
        const string insertSql = """
            INSERT INTO exposicoes (ativo_codigo, valor_atual, usuario_inclusao, data_inclusao, versao)
            VALUES (@Code, 0, 'system', NOW(), 1)
            ON CONFLICT (ativo_codigo) DO NOTHING;
            """;
        await connection.ExecuteAsync(new CommandDefinition(insertSql, new { Code = code }, transaction, cancellationToken: cancellationToken));

        // FOR UPDATE bloqueia a linha durante a transação para proteger a exposição atual.
        const string selectSql = """
            SELECT valor_atual
            FROM exposicoes
            WHERE ativo_codigo = @Code
            FOR UPDATE;
            """;
        var currentValue = await connection.QuerySingleAsync<decimal>(
            new CommandDefinition(selectSql, new { Code = code }, transaction, cancellationToken: cancellationToken));

        await transaction.CommitAsync(cancellationToken);
        return Exposure.Rehydrate(asset, currentValue);
    }

    public async Task SaveAsync(Exposure exposure, CancellationToken cancellationToken)
    {
        var code = ToCode(exposure.Asset);
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        // O UPDATE registra o novo valor e os dados de auditoria definidos na diagramção.
        const string updateSql = """
            UPDATE exposicoes
            SET valor_atual = @CurrentValue,
                usuario_alteracao = 'system',
                data_alteracao = NOW(),
                versao = versao + 1
            WHERE ativo_codigo = @Code;
            """;
        await connection.ExecuteAsync(new CommandDefinition(
            updateSql,
            new { Code = code, CurrentValue = exposure.CurrentValue },
            cancellationToken: cancellationToken));
    }

    // O domínio usa enum; o banco usa o código textual definido no contrato.
    private static string ToCode(Asset asset) => asset switch
    {
        Asset.Petr4 => "PETR4",
        Asset.Vale3 => "VALE3",
        Asset.Viia4 => "VIIA4",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Ativo não suportado.")
    };
}
