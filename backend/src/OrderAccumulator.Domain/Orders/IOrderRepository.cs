using OrderAccumulator.Domain.Exposures;

namespace OrderAccumulator.Domain.Orders;

// Porta de persistência para ordens aceitas, independente de Dapper ou PostgreSQL.
public interface IOrderRepository
{
    // Persiste a ordem e a exposição atual dentro da mesma transação.
    Task SaveAcceptedAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken);
}
