using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Domain.Exposures;

// Porta de persistência: o domínio não conhece memória, SQL ou qualquer banco específico.
public interface IExposureRepository
{
    Task<Exposure> GetOrCreateAsync(Asset asset, CancellationToken cancellationToken);
    Task SaveAsync(Exposure exposure, CancellationToken cancellationToken);
}

