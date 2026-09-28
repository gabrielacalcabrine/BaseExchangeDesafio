using OrderAccumulator.Domain.Exposures;

namespace OrderAccumulator.Domain.Orders;

public interface IOrderRepository
{
    Task<bool> SalvarOrdemAceitaAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken);
}
