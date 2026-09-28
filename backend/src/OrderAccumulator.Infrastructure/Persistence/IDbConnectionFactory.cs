using System.Data.Common;

namespace OrderAccumulator.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    Task<DbConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken);
}
