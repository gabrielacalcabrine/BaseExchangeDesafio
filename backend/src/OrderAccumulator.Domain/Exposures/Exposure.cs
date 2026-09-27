using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Domain.Exposures;

// Agregado responsável pela invariável do limite financeiro por ativo.
public sealed class Exposure
{
    public const decimal Limit = 1_000_000m;
    private Exposure(Asset asset) => Asset = asset;
    public Asset Asset { get; }
    public decimal CurrentValue { get; private set; }

    // Cria uma exposição vazia para um ativo.
    public static Exposure Empty(Asset asset) => new(asset);

    // Reconstitui o agregado a partir de um valor persistido, sem expor o construtor.
    public static Exposure Rehydrate(Asset asset, decimal currentValue)
        => new(asset) { CurrentValue = decimal.Round(currentValue, 2) };

    // Valida primeiro e só depois altera o estado, garantindo rejeição sem efeito parcial.
    public bool TryRegister(Order order, out string? error)
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
