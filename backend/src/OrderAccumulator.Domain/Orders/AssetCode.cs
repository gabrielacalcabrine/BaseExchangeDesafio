using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

public sealed record AssetCode
{
    private AssetCode(string value) => Value = value;
    public string Value { get; }

    public static AssetCode Criar(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (normalized is not ("PETR4" or "VALE3" or "VIIA4"))
            throw new DomainValidationException("O ativo deve ser PETR4, VALE3 ou VIIA4.");
        return new AssetCode(normalized);
    }

    public Asset ParaEnum() => Value switch
    {
        "PETR4" => Asset.Petr4,
        "VALE3" => Asset.Vale3,
        "VIIA4" => Asset.Viia4,
        _ => throw new DomainValidationException("Ativo não suportado.")
    };
}
