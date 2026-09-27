using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Application.Orders.Commands;

// Comando interno da aplicação. O caso de uso recebe tipos de domínio, não DTOs HTTP.
public sealed record CreateOrderCommand(Asset Asset, Side Side, int Quantity, decimal Price);
