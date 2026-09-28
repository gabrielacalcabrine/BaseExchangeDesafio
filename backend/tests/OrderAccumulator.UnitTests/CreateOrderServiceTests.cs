using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Services;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.UnitTests;

public sealed class CreateOrderServiceTests
{
    [Fact]
    public async Task ExecutarAsync_DevePersistirOrdemAceitaERetornarExposicao()
    {
        var exposureRepository = new FakeExposureRepository(Asset.Petr4);
        var orderRepository = new FakeOrderRepository { SaveResult = true };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecutarAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Buy, 100, 10.50m),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1_050m, result.CurrentExposure);
        Assert.True(orderRepository.WasCalled);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRejeitarOrdemSemPersistirQuandoLimiteForExcedido()
    {
        var exposureRepository = new FakeExposureRepository(Asset.Petr4, 999_999m);
        var orderRepository = new FakeOrderRepository { SaveResult = true };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecutarAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Buy, 2, 1m),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(999_999m, result.CurrentExposure);
        Assert.False(orderRepository.WasCalled);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarFalhaQuandoRepositoryDetectarConflitoDeConcorrencia()
    {
        var exposureRepository = new FakeExposureRepository(Asset.Petr4);
        var orderRepository = new FakeOrderRepository { SaveResult = false };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecutarAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Sell, 10, 5m),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("alterada por outra operação", result.ErrorMessage);
    }

    private sealed class FakeExposureRepository(Asset asset, decimal currentValue = 0) : IExposureRepository
    {
        private readonly Exposure exposure = Exposure.Reidratar(asset, currentValue);
        public Task<Exposure> ObterOuCriarAsync(Asset requestedAsset, CancellationToken cancellationToken)
            => Task.FromResult(exposure);
        public Task SalvarAsync(Exposure savedExposure, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public bool SaveResult { get; init; }
        public bool WasCalled { get; private set; }

        public Task<bool> SalvarOrdemAceitaAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult(SaveResult);
        }
    }
}
