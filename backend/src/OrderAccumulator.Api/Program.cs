using OrderAccumulator.Infrastructure;

// Composition Root: ponto único onde implementações concretas são registradas.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddExchangePersistence(builder.Configuration);
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
app.MapControllers();
app.Run();
