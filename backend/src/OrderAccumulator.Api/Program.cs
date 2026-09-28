using OrderAccumulator.Infrastructure;
using OrderAccumulator.Application.Orders.Services;
using OrderAccumulator.Application.Ports;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
DependencyInjection.AdicionarPersistencia(builder.Services, builder.Configuration);
builder.Services.AddScoped<IOrderAccumulator, CreateOrderService>();
builder.Services.AddScoped<FluentValidation.IValidator<OrderAccumulator.Api.Contracts.Orders.CreateOrderRequest>,
    OrderAccumulator.Api.Contracts.Orders.CreateOrderRequestValidator>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderAccumulator API v1"));
app.MapControllers();
app.Run();

public partial class Program { }
