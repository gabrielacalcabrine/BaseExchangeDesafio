using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;
using OrderAccumulator.Domain.Errors;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Api.Contracts.Orders;

// Mapper mantém a tradução entre nomes externos (português/códigos) e o domínio.
public static class OrderContractMapper
{
    public static bool TryToCommand(this CreateOrderRequest request, out CreateOrderCommand? command, out string? error)
    {
        command = null;
        error = null;

        try
        {
            var asset = AssetCode.Create(request.Ativo);
            var side = SideCode.Create(request.Lado);
            var quantity = OrderQuantity.Create(request.Quantidade);
            var price = OrderPrice.Create(request.Preco);
            command = new CreateOrderCommand(asset.ToEnum(), side.ToEnum(), quantity.Value, price.Value);
            return true;
        }
        catch (DomainValidationException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    // Mapeamento manual do resultado interno para o contrato HTTP de saída.
    public static CreateOrderResponse ToResponse(this CreateOrderResult result)
        => new(
            Sucesso: result.Success,
            ExposicaoAtual: result.CurrentExposure,
            MsgErro: result.ErrorMessage);
}
