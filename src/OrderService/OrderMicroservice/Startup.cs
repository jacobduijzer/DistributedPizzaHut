using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OrderMicroservice;

public class Startup
{
    const string zipkinEndpoint = "http://localhost:9411/api/v2/spans";


    private IConfiguration _configuration;

    public Startup(IConfiguration configuration) =>
        _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDbContext<OrderDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString("OrderDatabase")))
            .AddScoped<IOrderStorage, OrderStorage>()
            .AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<OrderToCreateEventConsumer>(typeof(OrderToCreateEventConsumerDefinition));
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost");
                    cfg.ConfigureEndpoints(context);
                });
            })
            .AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name))
                    .AddMassTransitInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter(with => with.Endpoint = new Uri(zipkinEndpoint));
            }).AddHostedService<ConsoleHostedService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrderDbContext orderDbContext)
    {
        orderDbContext.Database.EnsureCreated();
    }
}