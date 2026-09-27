using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using OrderAccumulator.Api.Contracts.Orders;
using OrderAccumulator.Application.Ports;

namespace OrderAccumulator.Api.Controllers;

// Adaptador HTTP: recebe o contrato externo e delegará ao caso de uso via DI.
[ApiController]
[Route("orders")]
public sealed class OrdersController(
    IValidator<CreateOrderRequest> validator,
    IOrderAccumulator orderAccumulator) : ControllerBase
{
    private readonly IValidator<CreateOrderRequest> _validator = validator;
    private readonly IOrderAccumulator _orderAccumulator = orderAccumulator;

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Criar(CreateOrderRequest request)
    {
        // ValidateAsync() executa as regras do FluentValidation e devolve um resultado agregado.
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            // Select() projeta cada erro para sua mensagem; string.Join() concatena as mensagens.
            var message = string.Join(" ", validation.Errors.Select(error => error.ErrorMessage));
            return BadRequest(new CreateOrderResponse(false, 0, message));
        }

        // TentarConverterParaComando() realiza o mapeamento manual request -> command.
        if (!OrderContractMapper.TentarConverterParaComando(request, out var command, out var error))
        {
            return BadRequest(new CreateOrderResponse(false, 0, error ?? "Requisição inválida."));
        }

        // O controller delega a regra ao caso de uso e converte apenas o resultado HTTP.
        var result = await _orderAccumulator.ExecutarAsync(command!, HttpContext.RequestAborted);
        var response = OrderContractMapper.ParaResposta(result);
        return result.Success ? Ok(response) : BadRequest(response);
    }
}
