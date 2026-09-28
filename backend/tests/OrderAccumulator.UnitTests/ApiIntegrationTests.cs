using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;

namespace OrderAccumulator.UnitTests;

public sealed class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AssetCode = "VALE3";
    private readonly HttpClient client;
    private readonly string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ExchangeDatabase")
        ?? "Host=localhost;Port=5432;Database=exchange;Username=exchange;Password=exchange";

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_DeveEstarDisponivel()
    {
        var response = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("swagger", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CriarOrdem_DeveRetornarContratoExatoDeResposta()
    {
        await RestaurarAtivoAsync();
        try
        {
            var request = new
            {
                ativo = AssetCode,
                lado = "C",
                quantidade = 10,
                preco = 12.34m
            };

            var response = await client.PostAsJsonAsync("/orders", request);
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(json.GetProperty("sucesso").GetBoolean());
            Assert.Equal(123.40m, json.GetProperty("exposicao_atual").GetDecimal());
            Assert.Equal(string.Empty, json.GetProperty("msg_erro").GetString());
        }
        finally
        {
            await RestaurarAtivoAsync();
        }
    }

    [Fact]
    public async Task CriarOrdem_DeveRetornarBadRequestQuandoLimiteDeExposicaoForExcedido()
    {
        await RestaurarAtivoAsync();
        try
        {
            var request = new
            {
                ativo = AssetCode,
                lado = "C",
                quantidade = 99_999,
                preco = 999.99m
            };

            var response = await client.PostAsJsonAsync("/orders", request);
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(json.GetProperty("sucesso").GetBoolean());
            Assert.Equal(0m, json.GetProperty("exposicao_atual").GetDecimal());
            Assert.Contains("limite", json.GetProperty("msg_erro").GetString(), StringComparison.OrdinalIgnoreCase);

            await using var connection = new NpgsqlConnection(connectionString);
            var orderCount = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ordens WHERE ativo_codigo = @AssetCode",
                new { AssetCode });
            Assert.Equal(0, orderCount);
        }
        finally
        {
            await RestaurarAtivoAsync();
        }
    }

    private async Task RestaurarAtivoAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM ordens WHERE ativo_codigo = @AssetCode; UPDATE exposicoes SET valor_atual = 0, versao = 1, usuario_alteracao = 'test-cleanup', data_alteracao = NOW() WHERE ativo_codigo = @AssetCode;", new { AssetCode });
    }
}
