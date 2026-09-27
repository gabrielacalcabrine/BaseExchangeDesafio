using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;

namespace OrderAccumulator.Application.Ports;

// Porta de entrada da aplicação: controllers, mensageria ou testes podem usá-la.
public interface IOrderAccumulator
{
    Task<CreateOrderResult> ExecuteAsync(CreateOrderCommand command, CancellationToken cancellationToken);
}
