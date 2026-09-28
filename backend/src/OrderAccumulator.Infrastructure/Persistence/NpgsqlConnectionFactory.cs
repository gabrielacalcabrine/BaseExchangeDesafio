using System.Data.Common;
using Npgsql;

namespace OrderAccumulator.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public async Task<DbConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
