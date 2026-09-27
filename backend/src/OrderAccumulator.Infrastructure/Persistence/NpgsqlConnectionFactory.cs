using System.Data.Common;
using Npgsql;

namespace OrderAccumulator.Infrastructure.Persistence;

// Adapter PostgreSQL: concentra a criação da conexão e mantém Npgsql fora do domínio.
public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        // NpgsqlConnection representa a conexão física com o PostgreSQL.
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
