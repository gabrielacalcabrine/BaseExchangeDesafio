using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;
using OrderAccumulator.Domain.Errors;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Api.Contracts.Orders;

public static class OrderContractMapper
{
    public static bool TentarConverterParaComando(CreateOrderRequest request, out CreateOrderCommand? command, out string? error)
    {
        command = null;
        error = null;

        try
        {
            var asset = AssetCode.Criar(request.Ativo);
            var side = SideCode.Criar(request.Lado);
            var quantity = OrderQuantity.Criar(request.Quantidade);
            var price = OrderPrice.Criar(request.Preco);

            command = new CreateOrderCommand(asset.ParaEnum(), side.ParaEnum(), quantity.Value, price.Value);
            return true;
        }
        catch (DomainValidationException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    public static CreateOrderResponse ParaResposta(CreateOrderResult result)
        => new(
            Sucesso: result.Success,
            ExposicaoAtual: result.CurrentExposure,
            MsgErro: result.ErrorMessage);
}
