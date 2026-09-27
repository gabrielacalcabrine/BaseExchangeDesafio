using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using OrderAccumulator.Api.Contracts.Orders;
using OrderAccumulator.Application.Ports;

namespace OrderAccumulator.Api.Controllers;

// Adaptador HTTP: recebe o contrato externo e delegará ao caso de uso via DI.
[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IValidator<CreateOrderRequest> validator;
    private readonly IOrderAccumulator orderAccumulator;

    public OrdersController(
        IValidator<CreateOrderRequest> validator,
        IOrderAccumulator orderAccumulator)
        => (this.validator, this.orderAccumulator) = (validator, orderAccumulator);

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(CreateOrderRequest request)
    {
        // ValidateAsync() executa as regras do FluentValidation e devolve um resultado agregado.
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            // Select() projeta cada erro para sua mensagem; string.Join() concatena as mensagens.
            var message = string.Join(" ", validation.Errors.Select(error => error.ErrorMessage));
            return BadRequest(new CreateOrderResponse(false, 0, message));
        }

        // TryToCommand() realiza o mapeamento manual request -> command.
        if (!request.TryToCommand(out var command, out var error))
        {
            return BadRequest(new CreateOrderResponse(false, 0, error ?? "Requisição inválida."));
        }

        // O controller delega a regra ao caso de uso e converte apenas o resultado HTTP.
        var result = await orderAccumulator.ExecuteAsync(command!, HttpContext.RequestAborted);
        var response = result.ToResponse();
        return result.Success ? Ok(response) : BadRequest(response);
    }
}
