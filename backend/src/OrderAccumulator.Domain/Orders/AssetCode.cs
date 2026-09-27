using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

// Value Object imutável que representa um ativo permitido pelo domínio.
public sealed record AssetCode
{
    private AssetCode(string value) => Value = value;
    public string Value { get; }

    public static AssetCode Create(string value)
    {
        // O operador ?. evita NullReferenceException se a entrada for nula.
        // Trim() remove espaços nas extremidades; ToUpperInvariant() normaliza o texto
        // sem depender da cultura regional da máquina.
        var normalized = value?.Trim().ToUpperInvariant();

        // O pattern "is not" verifica se o valor não pertence ao conjunto permitido.
        if (normalized is not ("PETR4" or "VALE3" or "VIIA4"))
            throw new DomainValidationException("O ativo deve ser PETR4, VALE3 ou VIIA4.");
        return new AssetCode(normalized);
    }

    // A switch expression converte o código externo para o enum interno do domínio.
    public Asset ToEnum() => Value switch
    {
        "PETR4" => Asset.Petr4,
        "VALE3" => Asset.Vale3,
        "VIIA4" => Asset.Viia4,
        _ => throw new DomainValidationException("Ativo não suportado.")
    };
}
