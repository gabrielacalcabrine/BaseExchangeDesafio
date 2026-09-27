namespace OrderAccumulator.Api.Contracts.Orders;

// DTO externo: representa exatamente o JSON devolvido ao OrderGenerator.
public sealed record CreateOrderResponse(bool Sucesso, decimal ExposicaoAtual, string MsgErro);
