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
    public void CodigoAtivo_DeveAceitarAtivosSuportados(string value)
    {
        // O Value Object deve aceitar somente os ativos previstos no contrato.
        var asset = AssetCode.Criar(value);

        Assert.Equal(value, asset.Value);
    }

    [Fact]
    public void CodigoAtivo_DeveNormalizarEntradaMinuscula()
    {
        // A representação interna deve ser consistente, mesmo que a entrada venha em minúsculas.
        var asset = AssetCode.Criar("petr4");

        Assert.Equal("PETR4", asset.Value);
    }

    [Fact]
    public void CodigoAtivo_DeveRejeitarAtivoNaoSuportado()
    {
        // Ativos fora do conjunto permitido não podem atravessar a fronteira do domínio.
        Assert.Throws<DomainValidationException>(() => AssetCode.Criar("ITUB4"));
    }

    [Theory]
    [InlineData("C", Side.Buy)]
    [InlineData("V", Side.Sell)]
    public void CodigoLado_DeveMapearCodigoDoContrato(string value, Side expected)
    {
        // O código externo deve ser convertido para o tipo usado pelo domínio.
        var side = SideCode.Criar(value);

        Assert.Equal(expected, side.ParaEnum());
    }

    [Fact]
    public void CodigoLado_DeveRejeitarLadoDesconhecido()
    {
        // Apenas compra e venda fazem parte do contrato.
        Assert.Throws<DomainValidationException>(() => SideCode.Criar("X"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99_999)]
    public void QuantidadeOrdem_DeveAceitarValoresDentroDosLimites(int value)
    {
        // Os limites são inclusivos apenas para valores estritamente positivos e menores que 100.000.
        var quantity = OrderQuantity.Criar(value);

        Assert.Equal(value, quantity.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100_000)]
    [InlineData(-1)]
    public void QuantidadeOrdem_DeveRejeitarValoresForaDosLimites(int value)
    {
        // Valores inválidos devem falhar antes de qualquer ordem ser criada.
        Assert.Throws<DomainValidationException>(() => OrderQuantity.Criar(value));
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(54.87)]
    [InlineData(999.99)]
    public void PrecoOrdem_DeveAceitarCentavosValidos(decimal value)
    {
        // O preço deve ser positivo, ter centavos válidos e permanecer abaixo de 1.000.
        var price = OrderPrice.Criar(value);

        Assert.Equal(value, price.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(54.871)]
    public void PrecoOrdem_DeveRejeitarValoresInvalidos(decimal value)
    {
        // Preços fora do intervalo ou com mais de duas casas não são aceitos.
        Assert.Throws<DomainValidationException>(() => OrderPrice.Criar(value));
    }
}
