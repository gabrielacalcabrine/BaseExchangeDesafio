using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

public sealed record SideCode
{
    private SideCode(string value) => Value = value;
    public string Value { get; }

    public static SideCode Criar(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (normalized is not ("C" or "V"))
            throw new DomainValidationException("O lado deve ser C para compra ou V para venda.");
        return new SideCode(normalized);
    }

    public Side ParaEnum() => Value == "C" ? Side.Buy : Side.Sell;
}
