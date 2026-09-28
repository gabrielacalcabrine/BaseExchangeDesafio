using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Application.Orders.Commands;

public sealed record CreateOrderCommand(Asset Asset, Side Side, int Quantity, decimal Price);
