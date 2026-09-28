using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;
using OrderAccumulator.Application.Ports;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Application.Orders.Services;

public sealed class CreateOrderService(
    IExposureRepository exposureRepository,
    IOrderRepository orderRepository) : IOrderAccumulator
{
    private readonly IExposureRepository _exposureRepository = exposureRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<CreateOrderResult> ExecutarAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = Order.Criar(command.Asset, command.Side, command.Quantity, command.Price);

        var exposure = await _exposureRepository.ObterOuCriarAsync(order.Asset, cancellationToken);
        if (!exposure.TentarRegistrar(order, out var error))
        {
            return CreateOrderResult.Falha(error ?? "A ordem excede o limite de exposição.", exposure.CurrentValue);
        }

        var saved = await _orderRepository.SalvarOrdemAceitaAsync(order, exposure, "system", cancellationToken);
        if (!saved)
        {
            return CreateOrderResult.Falha(
                "A exposição foi alterada por outra operação. Tente enviar a ordem novamente.");
        }

        return CreateOrderResult.Sucesso(exposure.CurrentValue);
    }
}
