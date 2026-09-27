namespace OrderAccumulator.Domain.Orders;

// Regras invariantes da ordem, mantidas no domínio para evitar duplicação entre adapters.
public static class OrderRules
{
    public const int MaximumQuantityExclusive = 100_000;
    public const decimal MaximumPriceExclusive = 1_000m;
    public const decimal PriceTick = 0.01m;
}
