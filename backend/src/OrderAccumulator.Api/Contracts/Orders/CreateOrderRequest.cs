namespace OrderAccumulator.Api.Contracts.Orders;

// DTO externo: representa exatamente o JSON recebido pela API.
public sealed record CreateOrderRequest(string Ativo, string Lado, int Quantidade, decimal Preco);
