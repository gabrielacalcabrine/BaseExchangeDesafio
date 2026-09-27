using FluentValidation;

namespace OrderAccumulator.Api.Contracts.Orders;

// Validação de entrada HTTP: fornece feedback estruturado antes de chegar ao caso de uso.
public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.Ativo)
            .Must(value => value is "PETR4" or "VALE3" or "VIIA4")
            .WithMessage("O ativo deve ser PETR4, VALE3 ou VIIA4.");
        RuleFor(request => request.Lado)
            .Must(value => value is "C" or "V")
            .WithMessage("O lado deve ser C para compra ou V para venda.");
        RuleFor(request => request.Quantidade)
            .GreaterThan(0)
            .LessThan(100_000)
            .WithMessage("A quantidade deve ser um inteiro positivo menor que 100.000.");
        RuleFor(request => request.Preco)
            .GreaterThan(0)
            .LessThan(1_000m)
            .Must(price => decimal.Round(price, 2) == price && price % 0.01m == 0)
            .WithMessage("O preço deve ser positivo, menor que 1.000 e múltiplo de 0,01.");
    }
}
