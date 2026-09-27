using OrderAccumulator.Domain.Errors;

namespace OrderAccumulator.Domain.Orders;

// Value Object para quantidade, garantindo inteiro positivo dentro do limite do desafio.
public sealed record OrderQuantity
{
    private OrderQuantity(int value) => Value = value;
    public int Value { get; }

    public static OrderQuantity Create(int value)
    {
        if (value <= 0 || value >= OrderRules.MaximumQuantityExclusive)
            throw new DomainValidationException("A quantidade deve ser positiva e menor que 100.000.");
        return new OrderQuantity(value);
    }
}
