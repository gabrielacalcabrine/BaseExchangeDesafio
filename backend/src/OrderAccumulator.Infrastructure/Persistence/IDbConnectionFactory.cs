using System.Data.Common;

namespace OrderAccumulator.Infrastructure.Persistence;

// Porta interna da infraestrutura para criar conexões abertas com o banco.
public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken);
}
