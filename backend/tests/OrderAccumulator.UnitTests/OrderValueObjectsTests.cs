using OrderAccumulator.Domain.Errors;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.UnitTests;

public sealed class OrderValueObjectsTests
{
    [Theory]
    [InlineData("PETR4")]
    [InlineData("VALE3")]
    [InlineData("VIIA4")]
    public void CodigoAtivo_DeveAceitarAtivosSuportados(string value)
    {
        var asset = AssetCode.Criar(value);

        Assert.Equal(value, asset.Value);
    }

    [Fact]
    public void CodigoAtivo_DeveNormalizarEntradaMinuscula()
    {
        var asset = AssetCode.Criar("petr4");

        Assert.Equal("PETR4", asset.Value);
    }

    [Fact]
    public void CodigoAtivo_DeveRejeitarAtivoNaoSuportado()
    {
        Assert.Throws<DomainValidationException>(() => AssetCode.Criar("ITUB4"));
    }

    [Theory]
    [InlineData("C", Side.Buy)]
    [InlineData("V", Side.Sell)]
    public void CodigoLado_DeveMapearCodigoDoContrato(string value, Side expected)
    {
        var side = SideCode.Criar(value);

        Assert.Equal(expected, side.ParaEnum());
    }

    [Fact]
    public void CodigoLado_DeveRejeitarLadoDesconhecido()
    {
        Assert.Throws<DomainValidationException>(() => SideCode.Criar("X"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99_999)]
    public void QuantidadeOrdem_DeveAceitarValoresDentroDosLimites(int value)
    {
        var quantity = OrderQuantity.Criar(value);

        Assert.Equal(value, quantity.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100_000)]
    [InlineData(-1)]
    public void QuantidadeOrdem_DeveRejeitarValoresForaDosLimites(int value)
    {
        Assert.Throws<DomainValidationException>(() => OrderQuantity.Criar(value));
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(54.87)]
    [InlineData(999.99)]
    public void PrecoOrdem_DeveAceitarCentavosValidos(decimal value)
    {
        var price = OrderPrice.Criar(value);

        Assert.Equal(value, price.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(54.871)]
    public void PrecoOrdem_DeveRejeitarValoresInvalidos(decimal value)
    {
        Assert.Throws<DomainValidationException>(() => OrderPrice.Criar(value));
    }
}
