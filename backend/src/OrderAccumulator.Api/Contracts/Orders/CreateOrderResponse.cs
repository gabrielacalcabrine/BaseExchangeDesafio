using System.Text.Json.Serialization;

namespace OrderAccumulator.Api.Contracts.Orders;

// DTO externo: representa exatamente o JSON devolvido ao OrderGenerator.
public sealed record CreateOrderResponse(
    [property: JsonPropertyName("sucesso")] bool Sucesso,
    [property: JsonPropertyName("exposicao_atual")] decimal ExposicaoAtual,
    [property: JsonPropertyName("msg_erro")] string MsgErro);
