using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

// Value Object para o código externo do lado da ordem.
public sealed record SideCode
{
    private SideCode(string value) => Value = value;
    public string Value { get; }

    public static SideCode Create(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        if (normalized is not ("C" or "V"))
            throw new DomainValidationException("O lado deve ser C para compra ou V para venda.");
        return new SideCode(normalized);
    }

    public Side ToEnum() => Value == "C" ? Side.Buy : Side.Sell;
}
