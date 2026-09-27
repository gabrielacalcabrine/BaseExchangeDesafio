using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

// Value Object para preço, usando decimal para evitar erros de ponto flutuante financeiro.
public sealed record OrderPrice
{
    private OrderPrice(decimal value) => Value = value;
    public decimal Value { get; }

    public static OrderPrice Create(decimal value)
    {
        if (value <= 0 || value >= OrderRules.MaximumPriceExclusive ||
            decimal.Round(value, 2) != value || value % OrderRules.PriceTick != 0)
            throw new DomainValidationException("O preço deve ser positivo, menor que 1.000 e múltiplo de 0,01.");
        return new OrderPrice(value);
    }
}
