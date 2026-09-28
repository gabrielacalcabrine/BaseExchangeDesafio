namespace OrderAccumulator.Domain.Errors;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message) { }
}
