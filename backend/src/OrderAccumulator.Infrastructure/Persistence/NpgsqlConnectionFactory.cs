using System.Data.Common;
using Npgsql;

namespace OrderAccumulator.Infrastructure.Persistence;

// Adapter PostgreSQL: concentra a criação da conexão e mantém Npgsql fora do domínio.
public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public async Task<DbConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken)
    {
        // NpgsqlConnection representa a conexão física com o PostgreSQL.
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
