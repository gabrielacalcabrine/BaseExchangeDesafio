using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using OrderAccumulator.Api.Contracts.Orders;

namespace OrderAccumulator.Api.Controllers;

// Adaptador HTTP: recebe o contrato externo e delegará ao caso de uso via DI.
[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IValidator<CreateOrderRequest> validator;

    public OrdersController(IValidator<CreateOrderRequest> validator)
        => this.validator = validator;

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

        // TryToCommand() é um método de extensão que realiza o mapeamento manual request -> command.
        // O mapper será mantido como adapter; o caso de uso será conectado na próxima etapa.
        if (!request.TryToCommand(out _, out var error))
        {
            return BadRequest(new CreateOrderResponse(false, 0, error ?? "Requisição inválida."));
        }
        return Ok(new CreateOrderResponse(false, 0, "Caso de uso ainda não implementado."));
    }
}
