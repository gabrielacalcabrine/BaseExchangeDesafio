using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Results;
using OrderAccumulator.Domain.Errors;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Api.Contracts.Orders;

// Mapper mantém a tradução entre nomes externos (português/códigos) e o domínio.
public static class OrderContractMapper
{
    public static bool TentarConverterParaComando(CreateOrderRequest request, out CreateOrderCommand? command, out string? error)
    {
        command = null;
        error = null;

        try
        {
            // Cada Criar() é uma factory method do Value Object e valida uma parte do contrato.
            var asset = AssetCode.Criar(request.Ativo);
            var side = SideCode.Criar(request.Lado);
            var quantity = OrderQuantity.Criar(request.Quantidade);
            var price = OrderPrice.Criar(request.Preco);

            // ParaEnum() converte os códigos externos para os enums usados pelo domínio.
            // Value acessa o valor primitivo já validado dentro de cada Value Object.
            command = new CreateOrderCommand(asset.ParaEnum(), side.ParaEnum(), quantity.Value, price.Value);
            return true;
        }
        catch (DomainValidationException exception)
        {
            // O catch transforma a exceção de domínio em erro controlado do adapter HTTP.
            error = exception.Message;
            return false;
        }
    }

    // Mapeamento manual do resultado interno para o contrato HTTP de saída.
    public static CreateOrderResponse ParaResposta(CreateOrderResult result)
        // A expressão new chama o construtor do record e associa cada campo explicitamente.
        => new(
            Sucesso: result.Success,
            ExposicaoAtual: result.CurrentExposure,
            MsgErro: result.ErrorMessage);
}
