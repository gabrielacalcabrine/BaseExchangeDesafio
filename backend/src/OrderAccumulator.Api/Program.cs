using OrderAccumulator.Infrastructure;
using OrderAccumulator.Application.Orders.Services;
using OrderAccumulator.Application.Ports;

// Composition Root: ponto único onde implementações concretas são registradas.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
DependencyInjection.AdicionarPersistencia(builder.Services, builder.Configuration);
builder.Services.AddScoped<IOrderAccumulator, CreateOrderService>();
// Registro explícito mantém o Composition Root claro e evita acoplamento a discovery automática.
builder.Services.AddScoped<FluentValidation.IValidator<OrderAccumulator.Api.Contracts.Orders.CreateOrderRequest>,
    OrderAccumulator.Api.Contracts.Orders.CreateOrderRequestValidator>();
// Mantém o JSON externo em camelCase, conforme o contrato compartilhado com o React.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
// Swagger fica disponível em todos os ambientes para facilitar o desenvolvimento e a demonstração.
app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderAccumulator API v1"));
app.MapControllers();
app.Run();

// Torna o bootstrap visível para WebApplicationFactory nos testes de integração da API.
public partial class Program { }
