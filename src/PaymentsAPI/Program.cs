using Api.Configurations.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Consumers;
using PaymentsAPI.Data;
using PaymentsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres");

// --- Database ---
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Application services ---
builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();

builder.Services.AddSwagger()
            .AddCorsPolicy()
            //.AddHealthCheck(builder.Environment)
            //.AddVersioning()
            .AddOptions(builder.Configuration);

// --- Messaging (MassTransit + RabbitMQ) ---
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    // Prefixo por serviço para filas próprias e nomes previsíveis.
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("payments", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseApiSwagger()
   .UseApiCorsPolicy()
   .UseApiHealthChecks();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "PaymentsAPI" }));
app.MapGet("/", () => Results.Ok(new { service = "PaymentsAPI", listening = "OrderPlacedEvent" }));

app.Run();
