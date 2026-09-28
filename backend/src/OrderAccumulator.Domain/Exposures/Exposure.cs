using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Domain.Exposures;

public sealed class Exposure
{
    public const decimal Limit = 1_000_000m;
    private Exposure(Asset asset) => Asset = asset;
    public Asset Asset { get; }
    public decimal CurrentValue { get; private set; }

    public static Exposure Vazia(Asset asset) => new(asset);

    public static Exposure Reidratar(Asset asset, decimal currentValue)
        => new(asset) { CurrentValue = decimal.Round(currentValue, 2) };

    public bool TentarRegistrar(Order order, out string? error)
    {
        var next = CurrentValue + order.SignedFinancialValue();
        if (Math.Abs(next) > Limit)
        {
            error = $"A exposição de {Asset} ultrapassaria o limite de R$ 1.000.000,00.";
            return false;
        }
        CurrentValue = decimal.Round(next, 2);
        error = null;
        return true;
    }
}
