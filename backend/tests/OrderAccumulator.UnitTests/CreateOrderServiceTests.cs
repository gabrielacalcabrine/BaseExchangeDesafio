using OrderAccumulator.Application.Orders.Commands;
using OrderAccumulator.Application.Orders.Services;
using OrderAccumulator.Domain.Exposures;
using OrderAccumulator.Domain.Orders;

namespace OrderAccumulator.UnitTests;

// Testes do caso de uso sem banco ou HTTP, usando portas falsas em memória.
public sealed class CreateOrderServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPersistAcceptedOrderAndReturnExposure()
    {
        // A exposição inicia zerada e a compra deve aumentar seu valor.
        var exposureRepository = new FakeExposureRepository(Asset.Petr4);
        var orderRepository = new FakeOrderRepository { SaveResult = true };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecuteAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Buy, 100, 10.50m),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1_050m, result.CurrentExposure);
        Assert.True(orderRepository.WasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectOrderWithoutPersistingWhenLimitIsExceeded()
    {
        // Uma exposição próxima do limite deve rejeitar a ordem que ultrapassaria R$ 1.000.000.
        var exposureRepository = new FakeExposureRepository(Asset.Petr4, 999_999m);
        var orderRepository = new FakeOrderRepository { SaveResult = true };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecuteAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Buy, 2, 1m),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(999_999m, result.CurrentExposure);
        Assert.False(orderRepository.WasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailureWhenRepositoryDetectsConcurrencyConflict()
    {
        // A porta de persistência pode sinalizar que outra operação atualizou o ativo primeiro.
        var exposureRepository = new FakeExposureRepository(Asset.Petr4);
        var orderRepository = new FakeOrderRepository { SaveResult = false };
        var service = new CreateOrderService(exposureRepository, orderRepository);

        var result = await service.ExecuteAsync(
            new CreateOrderCommand(Asset.Petr4, Side.Sell, 10, 5m),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("alterada por outra operação", result.ErrorMessage);
    }

    // Fake repository de exposição: substitui PostgreSQL sem alterar o caso de uso.
    private sealed class FakeExposureRepository(Asset asset, decimal currentValue = 0) : IExposureRepository
    {
        private readonly Exposure exposure = Exposure.Rehydrate(asset, currentValue);
        public Task<Exposure> GetOrCreateAsync(Asset requestedAsset, CancellationToken cancellationToken)
            => Task.FromResult(exposure);
        public Task SaveAsync(Exposure savedExposure, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    // Fake repository de ordens: registra se o caso de uso tentou persistir a operação.
    private sealed class FakeOrderRepository : IOrderRepository
    {
        public bool SaveResult { get; init; }
        public bool WasCalled { get; private set; }

        public Task<bool> SaveAcceptedAsync(Order order, Exposure exposure, string user, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult(SaveResult);
        }
    }
}
