namespace OrderAccumulator.Application.Orders.Results;

// Resultado interno da aplicação, independente do formato do transporte HTTP.
public sealed record CreateOrderResult(bool Success, decimal CurrentExposure, string ErrorMessage)
{
    // Factory para padronizar respostas de erro.
    public static CreateOrderResult Failure(string message, decimal currentExposure = 0)
        => new(false, currentExposure, message);

    // Factory para padronizar respostas de sucesso.
    public static CreateOrderResult Succeeded(decimal currentExposure)
        => new(true, currentExposure, string.Empty);
}
