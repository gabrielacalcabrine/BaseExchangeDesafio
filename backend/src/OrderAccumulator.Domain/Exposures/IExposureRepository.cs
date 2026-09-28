using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.Domain.Exposures;

public interface IExposureRepository
{
    Task<Exposure> ObterOuCriarAsync(Asset asset, CancellationToken cancellationToken);
    Task SalvarAsync(Exposure exposure, CancellationToken cancellationToken);
}
