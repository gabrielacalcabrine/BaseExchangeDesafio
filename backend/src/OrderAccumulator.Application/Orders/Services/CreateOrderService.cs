using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;
using OrderAccumulator.Application.Ports;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Application.Orders.Services;

// Caso de uso que orquestra domínio e portas sem conhecer HTTP, Dapper ou PostgreSQL.
public sealed class CreateOrderService(
    IExposureRepository exposureRepository,
    IOrderRepository orderRepository) : IOrderAccumulator
{
    public async Task<CreateOrderResult> ExecuteAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        // A entidade concentra a criação de uma ordem válida no domínio.
        var order = Order.Create(command.Asset, command.Side, command.Quantity, command.Price);

        // A exposição é carregada por ativo; cada ativo possui seu próprio limite.
        var exposure = await exposureRepository.GetOrCreateAsync(order.Asset, cancellationToken);
        if (!exposure.TryRegister(order, out var error))
        {
            // Ordem acima do limite não é enviada ao repository e não altera o estado persistido.
            return CreateOrderResult.Failure(error ?? "A ordem excede o limite de exposição.", exposure.CurrentValue);
        }

        // O repository grava ordem e exposição na mesma transação SQL.
        var saved = await orderRepository.SaveAcceptedAsync(order, exposure, "system", cancellationToken);
        if (!saved)
        {
            // Falha de concorrência: outro processo atualizou a exposição antes desta gravação.
            return CreateOrderResult.Failure(
                "A exposição foi alterada por outra operação. Tente enviar a ordem novamente.");
        }

        return CreateOrderResult.Succeeded(exposure.CurrentValue);
    }
}
