using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Domain.Exposures;

// Porta de persistência: o domínio não conhece memória, SQL ou qualquer banco específico.
public interface IExposureRepository
{
    Task<Exposure> ObterOuCriarAsync(Asset asset, CancellationToken cancellationToken);
    Task SalvarAsync(Exposure exposure, CancellationToken cancellationToken);
}
