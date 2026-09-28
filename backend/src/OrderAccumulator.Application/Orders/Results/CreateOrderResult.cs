namespace OrderAccumulator.Application.Orders.Results;

public sealed record CreateOrderResult(bool Success, decimal CurrentExposure, string ErrorMessage)
{
    public static CreateOrderResult Falha(string message, decimal currentExposure = 0)
        => new(false, currentExposure, message);

    public static CreateOrderResult Sucesso(decimal currentExposure)
        => new(true, currentExposure, string.Empty);
}
