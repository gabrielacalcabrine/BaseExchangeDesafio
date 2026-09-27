using OrderAccumulator.Domain.Errors;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.UnitTests;

// Testes unitários das invariantes mais básicas do domínio de ordens.
public sealed class OrderValueObjectsTests
{
    [Theory]
    [InlineData("PETR4")]
    [InlineData("VALE3")]
    [InlineData("VIIA4")]
    public void AssetCode_ShouldAcceptSupportedAssets(string value)
    {
        // O Value Object deve aceitar somente os ativos previstos no contrato.
        var asset = AssetCode.Create(value);

        Assert.Equal(value, asset.Value);
    }

    [Fact]
    public void AssetCode_ShouldNormalizeLowercaseInput()
    {
        // A representação interna deve ser consistente, mesmo que a entrada venha em minúsculas.
        var asset = AssetCode.Create("petr4");

        Assert.Equal("PETR4", asset.Value);
    }

    [Fact]
    public void AssetCode_ShouldRejectUnsupportedAsset()
    {
        // Ativos fora do conjunto permitido não podem atravessar a fronteira do domínio.
        Assert.Throws<DomainValidationException>(() => AssetCode.Create("ITUB4"));
    }

    [Theory]
    [InlineData("C", Side.Buy)]
    [InlineData("V", Side.Sell)]
    public void SideCode_ShouldMapContractCode(string value, Side expected)
    {
        // O código externo deve ser convertido para o tipo usado pelo domínio.
        var side = SideCode.Create(value);

        Assert.Equal(expected, side.ToEnum());
    }

    [Fact]
    public void SideCode_ShouldRejectUnknownSide()
    {
        // Apenas compra e venda fazem parte do contrato.
        Assert.Throws<DomainValidationException>(() => SideCode.Create("X"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99_999)]
    public void OrderQuantity_ShouldAcceptValuesInsideLimits(int value)
    {
        // Os limites são inclusivos apenas para valores estritamente positivos e menores que 100.000.
        var quantity = OrderQuantity.Create(value);

        Assert.Equal(value, quantity.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100_000)]
    [InlineData(-1)]
    public void OrderQuantity_ShouldRejectValuesOutsideLimits(int value)
    {
        // Valores inválidos devem falhar antes de qualquer ordem ser criada.
        Assert.Throws<DomainValidationException>(() => OrderQuantity.Create(value));
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(54.87)]
    [InlineData(999.99)]
    public void OrderPrice_ShouldAcceptValidCents(decimal value)
    {
        // O preço deve ser positivo, ter centavos válidos e permanecer abaixo de 1.000.
        var price = OrderPrice.Create(value);

        Assert.Equal(value, price.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(54.871)]
    public void OrderPrice_ShouldRejectInvalidValues(decimal value)
    {
        // Preços fora do intervalo ou com mais de duas casas não são aceitos.
        Assert.Throws<DomainValidationException>(() => OrderPrice.Create(value));
    }
}
