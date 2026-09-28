using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;

namespace OrderAccumulator.Application.Ports;

public interface IOrderAccumulator
{
    Task<CreateOrderResult> ExecutarAsync(CreateOrderCommand command, CancellationToken cancellationToken);
}
