namespace OrderAccumulator.Domain.Errors;

// Exceção para impedir a criação de objetos de domínio fora das invariantes.
public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message) { }
}
