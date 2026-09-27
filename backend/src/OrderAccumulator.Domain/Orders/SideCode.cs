using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

// Value Object para o código externo do lado da ordem.
public sealed record SideCode
{
    private SideCode(string value) => Value = value;
    public string Value { get; }

    public static SideCode Criar(string value)
    {
        // ?. protege contra nulo; Trim() remove espaços; ToUpperInvariant() padroniza o código.
        var normalized = value?.Trim().ToUpperInvariant();

        // O pattern "is not" rejeita qualquer código diferente de C ou V.
        if (normalized is not ("C" or "V"))
            throw new DomainValidationException("O lado deve ser C para compra ou V para venda.");
        return new SideCode(normalized);
    }

    // O operador ternário escolhe compra para C e venda para qualquer valor já validado como V.
    public Side ParaEnum() => Value == "C" ? Side.Buy : Side.Sell;
}
