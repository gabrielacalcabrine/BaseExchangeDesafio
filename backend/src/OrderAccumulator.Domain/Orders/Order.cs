namespace OrderAccumulator.Domain.Orders;

public sealed class Order
{
    private Order(Asset asset, Side side, int quantity, decimal price)
        => (Asset, Side, Quantity, Price) = (asset, side, quantity, price);

    public Asset Asset { get; }
    public Side Side { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    public static Order Criar(Asset asset, Side side, int quantity, decimal price)
    {
        _ = OrderQuantity.Criar(quantity);
        _ = OrderPrice.Criar(price);
        return new(asset, side, quantity, price);
    }

    public decimal SignedFinancialValue() => (Side == Side.Buy ? 1 : -1) * Quantity * Price;
}
