using System.Collections.Concurrent;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Infrastructure.Persistence;

// Implementação inicial para desenvolvimento, facilmente substituível por uma persistente.
public sealed class InMemoryExposureRepository : IExposureRepository
{
    private readonly ConcurrentDictionary<Asset, Exposure> exposures = new();
    public Task<Exposure> ObterOuCriarAsync(Asset asset, CancellationToken cancellationToken)
        => Task.FromResult(exposures.GetOrAdd(asset, Exposure.Vazia));
    public Task SalvarAsync(Exposure exposure, CancellationToken cancellationToken)
    {
        exposures[exposure.Asset] = exposure;
        return Task.CompletedTask;
    }
}
