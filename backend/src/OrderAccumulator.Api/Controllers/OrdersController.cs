using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using OrderAccumulator.Api.Contracts.Orders;
using OrderAccumulator.Application.Ports;

namespace OrderAccumulator.Api.Controllers;

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
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(error => error.ErrorMessage));
            return BadRequest(new CreateOrderResponse(false, 0, message));
        }

        if (!OrderContractMapper.TentarConverterParaComando(request, out var command, out var error))
        {
            return BadRequest(new CreateOrderResponse(false, 0, error ?? "Requisição inválida."));
        }

        var result = await _orderAccumulator.ExecutarAsync(command!, HttpContext.RequestAborted);
        var response = OrderContractMapper.ParaResposta(result);
        return result.Success ? Ok(response) : BadRequest(response);
    }
}
