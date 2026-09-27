namespace OrderAccumulator.Domain.Orders;

// Entidade de domínio para uma ordem normalizada e válida.
public sealed class Order
{
    private Order(Asset asset, Side side, int quantity, decimal price)
        => (Asset, Side, Quantity, Price) = (asset, side, quantity, price);

    public Asset Asset { get; }
    public Side Side { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    // Factory method centraliza a criação e deixa validações futuras em um só lugar.
    public static Order Criar(Asset asset, Side side, int quantity, decimal price)
    {
        // Os Value Objects protegem a entidade mesmo quando ela é criada fora do adapter HTTP.
        _ = OrderQuantity.Criar(quantity);
        _ = OrderPrice.Criar(price);
        return new(asset, side, quantity, price);
    }

    // Valor com sinal usado para atualizar a exposição do ativo.
    public decimal SignedFinancialValue() => (Side == Side.Buy ? 1 : -1) * Quantity * Price;
}
