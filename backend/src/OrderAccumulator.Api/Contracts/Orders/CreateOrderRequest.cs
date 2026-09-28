namespace OrderAccumulator.Api.Contracts.Orders;

public sealed record CreateOrderRequest(string Ativo, string Lado, int Quantidade, decimal Preco);
